using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkTimerController : MonoBehaviour
{
    [Header("UI Elements")]
    public List<GameObject> blinkObjects = new List<GameObject>();
    public Slider countdownSlider;
    public Slider activeTimeSlider;

    [Header("Timing Settings")]
    public float totalTime = 30f;
    public Vector2 randomCycleRange = new Vector2(20f, 40f);
    public float preBlinkTime = 6f;
    public float activeTime = 5f;
    public float blinkOnTime = 1f;
    public float blinkOffTime = 1f;

    [Header("Player Settings")]
    [Tooltip("Tag of the player object to affect")]
    public string playerTag = "Player";

    private float countdownTimer;
    private float activeTimer;
    private bool isBlinking = false;
    private bool isActive = false;
    private int currentBlinkIndex = 0;

    private Coroutine activePhaseLoop; // 🔁 Reference to active loop coroutine

    private void Start()
    {
        StartNewCycle();
    }

    private void Update()
    {
        if (!isActive)
        {
            HandleCountdownPhase();
        }
        else
        {
            HandleActivePhase();
        }
    }

    private void HandleCountdownPhase()
    {
        countdownTimer -= Time.deltaTime;

        if (countdownSlider != null)
            countdownSlider.value = countdownTimer;

        // Start blinking when close to activation
        if (!isBlinking && countdownTimer <= preBlinkTime)
        {
            StartCoroutine(BlinkSequence());
            isBlinking = true;
        }

        // Trigger activation
        if (countdownTimer <= 0)
        {
            ActivationEvent();
        }
    }

    private void HandleActivePhase()
    {
        activeTimer -= Time.deltaTime;

        if (activeTimeSlider != null)
            activeTimeSlider.value = activeTimer;

        if (activeTimer <= 0)
        {
            EndActivePhase();
        }
    }

    private void StartNewCycle()
    {
        totalTime = Random.Range(randomCycleRange.x, randomCycleRange.y);
        countdownTimer = totalTime;
        activeTimer = activeTime;
        isBlinking = false;
        isActive = false;
        currentBlinkIndex = 0;

        if (countdownSlider != null)
        {
            countdownSlider.minValue = 0f;
            countdownSlider.maxValue = totalTime;
            countdownSlider.value = totalTime;
        }

        if (activeTimeSlider != null)
        {
            activeTimeSlider.minValue = 0f;
            activeTimeSlider.maxValue = activeTime;
            activeTimeSlider.value = activeTime;
        }

        SetAllBlinkObjectsActive(false);
    }

    private IEnumerator BlinkSequence()
    {
        while (!isActive)
        {
            if (blinkObjects.Count == 0)
                yield break;

            SetAllBlinkObjectsActive(false);
            blinkObjects[currentBlinkIndex].SetActive(true);
            yield return new WaitForSeconds(blinkOnTime);

            SetAllBlinkObjectsActive(false);
            yield return new WaitForSeconds(blinkOffTime);

            currentBlinkIndex = (currentBlinkIndex + 1) % blinkObjects.Count;
        }
    }

    private void ActivationEvent()
    {
        Debug.Log("🔔 Activation triggered!");
        isActive = true;
        StopAllCoroutines(); // stop blinking
        SetAllBlinkObjectsActive(true);

        // Reset active timer
        activeTimer = activeTime;
        if (activeTimeSlider != null)
            activeTimeSlider.value = activeTimer;

        // 🔁 Start listening for input that kills player
        if (activePhaseLoop != null)
            StopCoroutine(activePhaseLoop);
        activePhaseLoop = StartCoroutine(ActivePhaseInputLoop());
    }

    private void EndActivePhase()
    {
        Debug.Log("🔁 Active time ended, restarting cycle.");
        isActive = false;

        // Stop input loop
        if (activePhaseLoop != null)
        {
            StopCoroutine(activePhaseLoop);
            activePhaseLoop = null;
        }

        SetAllBlinkObjectsActive(false);
        StartNewCycle();
    }

    /// <summary>
    /// 🔁 Runs while active phase is ongoing.
    /// Checks for player input and triggers death if detected.
    /// </summary>
    private IEnumerator ActivePhaseInputLoop()
    {
        Debug.Log("🎯 Input detection loop started.");

        while (isActive)
        {
            // Check for any key or button press
            if (Input.anyKeyDown)
            {
                Debug.Log("💀 Player pressed a button during active phase — triggering death!");
                TriggerPlayerDeath();
                break; // End loop after triggering death
            }

            yield return null; // wait for next frame
        }

        Debug.Log("⛔ Input detection loop ended.");
    }

    /// <summary>
    /// Finds the player and triggers their death handler.
    /// </summary>
    private void TriggerPlayerDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            PlayerDeathHandler deathHandler = player.GetComponent<PlayerDeathHandler>();
            if (deathHandler != null)
            {
                StartCoroutine(deathHandler.HandleDeath());
            }
            else
            {
                Debug.LogWarning("⚠️ Player does not have a PlayerDeathHandler component!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No GameObject with tag '" + playerTag + "' found!");
        }
    }

    private void SetAllBlinkObjectsActive(bool active)
    {
        foreach (var obj in blinkObjects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}
