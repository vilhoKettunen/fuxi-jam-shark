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

    [Header("Position Offset")]
    [Tooltip("Offset from the player's position (e.g., to be above the player).")]
    public Vector3 playerOffset = new Vector3(0f, 1f, 0f);

    [Tooltip("How fast the Y offset transitions when changed.")]
    public float offsetLerpSpeed = 3f;

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

    // Smooth transition target
    private float targetYOffset;

    // Debug cycle values
    private readonly float[] debugHeights = { 4f, 40f };
    private int currentDebugIndex = 0;

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
        targetYOffset = playerOffset.y;
    }

    void Update()
    {
        // 🔹 Debug control — press L to cycle between preset Y offsets
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentDebugIndex = (currentDebugIndex + 1) % debugHeights.Length;
            float newValue = debugHeights[currentDebugIndex];
            SetPlayerYOffset(newValue);
            Debug.Log($"[EyeFollowPlayer_SmartSmooth] Debug Y Offset changed to: {newValue}");
        }
    }

    void LateUpdate()
    {
        // Smoothly interpolate current Y offset toward target
        playerOffset.y = Mathf.Lerp(playerOffset.y, targetYOffset, Time.deltaTime * offsetLerpSpeed);

        // Track player's vertical movement
        verticalVelocity = (player.position.y - previousPlayerPosition.y) / Time.deltaTime;
        previousPlayerPosition = player.position;

        // Target position with horizontal, vertical follow, and offset
        Vector3 targetPos = new(
            initialPosition.x + (player.position.x - initialPosition.x) * horizontalFollowAmount + playerOffset.x,
            initialPosition.y + (player.position.y - initialPosition.y) * verticalFollowAmount + playerOffset.y,
            initialPosition.z + playerOffset.z
        );

        // Distance between eye and target
        float distance = Vector3.Distance(transform.position, targetPos);

        // Adaptive smooth time
        float smoothTime = baseSmoothTime;
        if (distance > catchUpDistance)
            smoothTime *= 0.5f;
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

        transform.LookAt(player);
    }

    /// <summary>
    /// Smoothly sets the Y offset of the eye relative to the player.
    /// Only values between 4 and 50 are accepted.
    /// Returns true if successfully set, false if out of range.
    /// </summary>
    public bool SetPlayerYOffset(float newYOffset)
    {
        if (newYOffset < 4f || newYOffset > 50f)
        {
            Debug.LogWarning($"EyeFollowPlayer_SmartSmooth: Tried to set invalid Y offset {newYOffset}. Must be between 4 and 50.");
            return false;
        }

        targetYOffset = newYOffset;
        return true;
    }
}
