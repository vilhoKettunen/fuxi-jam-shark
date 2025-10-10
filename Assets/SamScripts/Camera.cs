using UnityEngine;

public class SmoothCameraFollowWithLookAhead : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;          // Player to follow
    public float smoothTime = 0.3f;   // How smoothly the camera follows
    public Vector3 offset;            // Base offset from player
    public float lookAheadDistance = 2f;  // How far to look ahead based on movement
    public float lookAheadSmoothTime = 0.3f; // Smooth time for look-ahead

    private float velocityX = 0f;
    private float currentLookAheadX = 0f;
    private float targetLookAheadX = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Determine look-ahead based on player's horizontal input
        float inputX = Input.GetAxisRaw("Horizontal");
        targetLookAheadX = inputX * lookAheadDistance;

        // Smoothly interpolate look-ahead
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAheadX, lookAheadSmoothTime * Time.deltaTime);

        // Desired camera X position
        float targetX = target.position.x + offset.x + currentLookAheadX;

        // Smoothly move camera on X axis
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothTime);

        // Keep Y and Z using offset
        transform.position = new Vector3(newX, target.position.y + offset.y, target.position.z + offset.z);
    }
}
