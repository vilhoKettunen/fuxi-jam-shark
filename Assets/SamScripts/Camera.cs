using UnityEngine;
using System.Collections;

public class SmoothCameraFollowWithLookAhead : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;                  // Player to follow
    public float smoothTime = 0.3f;           // How smoothly the camera follows
    public Vector3 offset;                    // Base offset from player
    public float lookAheadDistance = 2f;      // How far to look ahead based on movement
    public float lookAheadSmoothTime = 0.3f;  // Smooth time for look-ahead

    // Shake / FOV
    private Camera cam;
    private float originalFOV;
    private bool isShaking = false;
    private Coroutine fovCoroutine;

    private float velocityX = 0f;
    private float currentLookAheadX = 0f;
    private float targetLookAheadX = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        originalFOV = cam.fieldOfView;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Determine look-ahead based on horizontal velocity
        float inputX = 0f;
        if (target.TryGetComponent<PlayerController>(out var player))
        {
            inputX = player.velocity.x;
        }

        targetLookAheadX = inputX * lookAheadDistance;

        // Smoothly interpolate look-ahead
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAheadX, lookAheadSmoothTime * Time.deltaTime);

        // Desired camera X position
        float targetX = target.position.x + offset.x + currentLookAheadX;

        // Smoothly move camera on X axis
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothTime);

        // Keep Y and Z using offset
        Vector3 followPos = new Vector3(newX, target.position.y + offset.y, target.position.z + offset.z);

        // Apply shake offset if shaking
        if (isShaking)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * currentShakeMagnitude;
            shakeOffset.z = 0;
            followPos += shakeOffset;
        }

        transform.position = followPos;
    }

    #region Camera Shake
    private float currentShakeMagnitude = 0f;

    public void ShakeCamera(float duration, float magnitude)
    {
        if (!isShaking)
            StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        isShaking = true;
        currentShakeMagnitude = magnitude;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
        currentShakeMagnitude = 0f;
    }
    #endregion

    #region FOV Zoom
    public void ZoomFOV(float amount, float duration)
    {
        if (fovCoroutine != null)
            StopCoroutine(fovCoroutine);

        fovCoroutine = StartCoroutine(FOVRoutine(amount, duration));
    }

    private IEnumerator FOVRoutine(float amount, float duration)
    {
        float elapsed = 0f;
        float startFOV = cam.fieldOfView;
        float targetFOV = startFOV - amount;

        // Zoom in
        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = targetFOV;

        // Zoom back out
        elapsed = 0f;
        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(targetFOV, originalFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = originalFOV;
        fovCoroutine = null;
    }
    #endregion
}
