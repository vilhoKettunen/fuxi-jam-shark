using UnityEngine;

public class EyeFallTrigger : MonoBehaviour
{
    [Header("Eye Settings")]
    public Transform eye;

    [Header("Player Settings")]
    public string playerTag = "Player";

    [Header("Fall Settings")]
    public float fallSpeed = 5f;
    public float fallDistance = 10f;
    public bool destroyAfterFall = true;
    public float destroyDelay = 1f;

    private bool isFalling = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Update()
    {
        if (!isFalling) return;

        // Move eye down toward target position
        eye.position = Vector3.MoveTowards(eye.position, targetPosition, fallSpeed * Time.deltaTime);

        // Keep looking at the player while falling
        if (eye.TryGetComponent<EyeFollowPlayer_SmartSmooth>(out var follow) && follow.player != null)
        {
            eye.LookAt(follow.player);
        }

        // Check if the eye reached the target position
        if (Vector3.Distance(eye.position, targetPosition) < 0.01f)
        {
            isFalling = false;

            // Destroy the eye if required
            if (destroyAfterFall)
            {
                Destroy(eye.gameObject, destroyDelay);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFalling) return;

        // Only react to the player
        if (other.CompareTag(playerTag))
        {
            StartFall();
        }
    }

    private void StartFall()
    {
        isFalling = true;

        // Record start position
        startPosition = eye.position;

        // Target position is full fallDistance below current position
        targetPosition = startPosition - new Vector3(0, fallDistance, 0);

        // Optionally, we could disable the SmoothFollow script to prevent it from pushing the eye
        if (eye.TryGetComponent<EyeFollowPlayer_SmartSmooth>(out var follow))
        {
            follow.enabled = false;
        }
    }
}
