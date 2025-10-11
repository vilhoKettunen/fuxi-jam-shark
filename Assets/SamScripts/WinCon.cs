using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class LaunchPadPrefabReady : MonoBehaviour
{
    [Header("Player Launch Settings")]
    [Tooltip("Upward force applied to the player instantly.")]
    public float launchForce = 15f;

    [Header("Audio Settings")]
    [Tooltip("Sound to play when player touches the launch pad.")]
    public AudioClip launchSound;

    [Header("Camera Effects")]
    [Tooltip("Amount to change the camera FOV.")]
    public float fovZoomAmount = 15f;
    [Tooltip("Duration of the FOV change in seconds.")]
    public float fovZoomDuration = 0.3f;
    [Tooltip("Duration of camera shake.")]
    public float shakeDuration = 0.3f;
    [Tooltip("Strength of camera shake.")]
    public float shakeMagnitude = 0.3f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Configure AudioSource for 2D playback
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // 2D sound
        audioSource.volume = 1f;

        // Ensure collider is trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

        private void PlayLaunchSound()
    {
        if (launchSound == null || audioSource == null) return;

        audioSource.Stop();                  // Ensure no previous sound conflicts
        audioSource.PlayOneShot(launchSound, 1f); // Play clip at full volume
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Launch the player instantly
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            if (other.TryGetComponent<CharacterController>(out var cc))
            {
                // Optional: immediate visual displacement
                cc.Move(Vector3.up * launchForce * Time.deltaTime);
            }

            // Set upward velocity for physics / continued motion
            player.velocity.y = launchForce;
        }

        

        // Trigger camera effects immediately
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.TryGetComponent<SmoothCameraFollowWithLookAhead>(out var camScript))
        {
            camScript.ShakeCamera(shakeDuration, shakeMagnitude);
            camScript.ZoomFOV(fovZoomAmount, fovZoomDuration);
        }

        // Destroy the launch pad immediately
        Destroy(gameObject);
    }

   
}
