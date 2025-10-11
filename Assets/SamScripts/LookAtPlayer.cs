using UnityEngine;

[RequireComponent(typeof(Transform))]
public class EyeFollowPlayer_SmartSmooth : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player transform the eye should follow.")]
    public Transform player;

    [Header("Follow Settings")]
    [Tooltip("Base smooth time (lower = tighter, higher = smoother).")]
    public float baseSmoothTime = 0.15f;

    [Tooltip("Horizontal follow amount (0 = no movement, 1 = fully follow).")]
    public float horizontalFollowAmount = 0.8f;

    [Tooltip("Vertical follow amount (0 = no movement, 1 = fully follow).")]
    public float verticalFollowAmount = 0.8f;

    [Tooltip("How much faster the eye follows when player is falling.")]
    public float fallSpeedMultiplier = 1.5f;

    [Tooltip("Distance at which the eye begins catching up faster.")]
    public float catchUpDistance = 1.0f;

    [Tooltip("Maximum speed the eye can move (to prevent snapping).")]
    public float maxFollowSpeed = 100f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 initialPosition;
    private Vector3 previousPlayerPosition;
    private float verticalVelocity;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("EyeFollowPlayer_SmartSmooth: No player assigned!");
            enabled = false;
            return;
        }

        initialPosition = transform.position;
        previousPlayerPosition = player.position;
    }

    void LateUpdate()
    {
        // Track player's vertical movement
        verticalVelocity = (player.position.y - previousPlayerPosition.y) / Time.deltaTime;
        previousPlayerPosition = player.position;

        // Target position with horizontal and vertical follow
        Vector3 targetPos = new Vector3(
            initialPosition.x + (player.position.x - initialPosition.x) * horizontalFollowAmount,
            initialPosition.y + (player.position.y - initialPosition.y) * verticalFollowAmount,
            initialPosition.z
        );

        // Distance between eye and target
        float distance = Vector3.Distance(transform.position, targetPos);

        // Adaptive smooth time
        float smoothTime = baseSmoothTime;

        if (distance > catchUpDistance)
            smoothTime *= 0.5f; // tighten catch-up

        if (verticalVelocity < -0.1f)
            smoothTime /= fallSpeedMultiplier;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime,
            maxFollowSpeed
        );

        // Always look at the player
        transform.LookAt(player);
    }
}
