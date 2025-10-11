using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Launch : MonoBehaviour
{
    [Header("Player Launch Settings")]
    public float launchForce = 15f;

    [Header("Audio Settings")]
    public AudioClip launchSound;

    [Header("Camera Effects")]
    public float fovZoomAmount = -15f;       // Negative to zoom in
    public float fovZoomDuration = 0.3f;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.3f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void PlayLaunchSound()
    {
        if (launchSound == null || audioSource == null) return;
        audioSource.Stop();
        audioSource.PlayOneShot(launchSound, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Launch player
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            if (other.TryGetComponent<CharacterController>(out var cc))
                cc.Move(Vector3.up * launchForce * Time.deltaTime);

            player.velocity.y = launchForce;
        }

        // Play audio
        PlayLaunchSound();

        // Camera effects
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // Zoom in temporarily
            StartCoroutine(TemporaryFOV(mainCam, fovZoomAmount, fovZoomDuration));

            // Shake if camera script supports it
            if (mainCam.TryGetComponent<SmoothCameraFollowWithLookAhead>(out var camScript))
                camScript.ShakeCamera(shakeDuration, shakeMagnitude);
        }
    }

    private IEnumerator TemporaryFOV(Camera cam, float fovChange, float duration)
    {
        float originalFOV = cam.fieldOfView;
        float targetFOV = originalFOV + fovChange;
        float elapsed = 0f;

        // Zoom in
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, elapsed / duration);
            yield return null;
        }

        cam.fieldOfView = targetFOV;

        // Smoothly return to original FOV
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(targetFOV, originalFOV, elapsed / duration);
            yield return null;
        }

        cam.fieldOfView = originalFOV;
    }
}
