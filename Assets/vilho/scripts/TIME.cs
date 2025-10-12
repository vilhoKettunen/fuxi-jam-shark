using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Colored UI blocks that will blink before activation.")]
    public List<GameObject> blinkObjects = new List<GameObject>();
    public Slider countdownSlider;
    public Slider activeTimeSlider;

    [Header("Timing Settings")]
    [Tooltip("Total time (in seconds) until activation.")]
    public float totalTime = 30f;
    [Tooltip("Random time range for the next cycle (min & max).")]
    public Vector2 randomCycleRange = new Vector2(20f, 40f);
    [Tooltip("Time (in seconds) before activation when blinking starts.")]
    public float preBlinkTime = 6f;
    [Tooltip("How long the system stays 'active' after activation (seconds).")]
    public float activeTime = 5f;
    [Tooltip("How long each color stays ON during blinking.")]
    public float blinkOnTime = 1f;
    [Tooltip("How long all colors stay OFF during blinking.")]
    public float blinkOffTime = 1f;

    [Header("Player Settings")]
    [Tooltip("Tag of the player object to affect.")]
    public string playerTag = "Player";

    [Header("Eye Settings")]
    [Tooltip("Drag your Eye GameObject here (must have EyeFollowPlayer_SmartSmooth component).")]
    public GameObject eyeObject;
    [Tooltip("Offset value when active phase starts.")]
    public float activeEyeOffset = 4f;
    [Tooltip("Offset value when active phase ends.")]
    public float defaultEyeOffset = 50f;

    // Cached reference to the eye follow script
    private LookAtPlayer eyeFollow;

    private float countdownTimer;
    private float activeTimer;
    private bool isBlinking = false;
    private bool isActive = false;
    private int currentBlinkIndex = 0;

    private Coroutine activePhaseLoop;

    private void Start()
    {
        // Auto-get the eye follow script
        if (eyeObject != null)
        {
            eyeFollow = eyeObject.GetComponent<LookAtPlayer>();
            if (eyeFollow == null)
                Debug.LogWarning("⚠️ The Eye Object does not have an EyeFollowPlayer_SmartSmooth component!");
        }
        else
        {
            Debug.LogWarning("⚠️ No Eye Object assigned in TimeController!");
        }

        StartNewCycle();
    }

    private void Update()
    {
        if (!isActive)
            HandleCountdownPhase();
        else
            HandleActivePhase();
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
            ActivationEvent();
    }

    private void HandleActivePhase()
    {
        activeTimer -= Time.deltaTime;

        if (activeTimeSlider != null)
            activeTimeSlider.value = activeTimer;

        if (activeTimer <= 0)
            EndActivePhase();
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
            if (blinkObjects == null || blinkObjects.Count == 0)
                yield break;

            SetAllBlinkObjectsActive(false);

            if (blinkObjects[currentBlinkIndex] != null)
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

        // Set eye offset
        if (eyeFollow != null)
        {
            eyeFollow.SetPlayerYOffset(activeEyeOffset);
            Debug.Log($"👁️ Eye offset set to {activeEyeOffset}");
        }

        // Start input detection loop
        if (activePhaseLoop != null)
            StopCoroutine(activePhaseLoop);
        activePhaseLoop = StartCoroutine(ActivePhaseInputLoop());
    }

    private void EndActivePhase()
    {
        Debug.Log("🔁 Active time ended, restarting cycle.");
        isActive = false;

        if (activePhaseLoop != null)
        {
            StopCoroutine(activePhaseLoop);
            activePhaseLoop = null;
        }

        // Reset eye offset
        if (eyeFollow != null)
        {
            eyeFollow.SetPlayerYOffset(defaultEyeOffset);
            Debug.Log($"👁️ Eye offset reset to {defaultEyeOffset}");
        }

        SetAllBlinkObjectsActive(false);
        StartNewCycle();
    }

    private IEnumerator ActivePhaseInputLoop()
    {
        Debug.Log("🎯 Input detection loop started.");

        while (isActive)
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("💀 Player pressed a button during active phase — triggering death!");
                TriggerPlayerDeath();
                break;
            }
            yield return null;
        }

        Debug.Log("⛔ Input detection loop ended.");
    }

    private void TriggerPlayerDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogWarning($"⚠️ No GameObject with tag '{playerTag}' found!");
            return;
        }

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

    private void SetAllBlinkObjectsActive(bool active)
    {
        if (blinkObjects == null) return;

        foreach (GameObject obj in blinkObjects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}
