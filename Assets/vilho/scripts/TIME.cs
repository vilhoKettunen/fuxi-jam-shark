using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkTimerController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Colored UI blocks that will blink before activation.")]
    public List<GameObject> blinkObjects = new List<GameObject>();

    [Tooltip("Slider showing countdown until activation.")]
    public Slider countdownSlider;

    [Tooltip("Slider showing remaining active time.")]
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

    private float countdownTimer;
    private float activeTimer;
    private bool isBlinking = false;
    private bool isActive = false;
    private int currentBlinkIndex = 0;

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

        // Update countdown slider
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
        // Pick a random total time for this cycle
        totalTime = Random.Range(randomCycleRange.x, randomCycleRange.y);
        countdownTimer = totalTime;
        activeTimer = activeTime;
        isBlinking = false;
        isActive = false;
        currentBlinkIndex = 0;

        // Update sliders
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

        // Make sure all are off
        SetAllBlinkObjectsActive(false);
    }

    private IEnumerator BlinkSequence()
    {
        while (!isActive)
        {
            if (blinkObjects.Count == 0)
                yield break;

            // Turn off all first
            SetAllBlinkObjectsActive(false);

            // Turn on current
            blinkObjects[currentBlinkIndex].SetActive(true);
            yield return new WaitForSeconds(blinkOnTime);

            // Turn all off
            SetAllBlinkObjectsActive(false);
            yield return new WaitForSeconds(blinkOffTime);

            // Move to next color
            currentBlinkIndex = (currentBlinkIndex + 1) % blinkObjects.Count;
        }
    }

    private void ActivationEvent()
    {
        Debug.Log("🔔 Activation triggered!");
        isActive = true;
        StopAllCoroutines(); // stop blinking
        SetAllBlinkObjectsActive(true);

        // Reset active timer and slider
        activeTimer = activeTime;
        if (activeTimeSlider != null)
            activeTimeSlider.value = activeTimer;
    }

    private void EndActivePhase()
    {
        Debug.Log("🔁 Active time ended, restarting cycle.");
        SetAllBlinkObjectsActive(false);
        StartNewCycle();
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