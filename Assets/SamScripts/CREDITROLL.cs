using UnityEngine;

public class SpriteSlowRise : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed at which the sprite moves upward (units per second).")]
    public float riseSpeed = 1f;

    [Tooltip("Maximum Y position to reach (optional). Set to 0 to ignore).")]
    public float maxY = 0f;

    [Tooltip("Start moving immediately on play.")]
    public bool autoStart = true;

    private bool isMoving = false;

    void Start()
    {
        if (autoStart)
            isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        // Move upward over time
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Optional: stop at max height
        if (maxY > 0f && transform.position.y >= maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            isMoving = false;
        }
    }

    /// <summary>
    /// Starts the upward movement.
    /// </summary>
    public void StartMoving()
    {
        isMoving = true;
    }

    /// <summary>
    /// Stops the upward movement.
    /// </summary>
    public void StopMoving()
    {
        isMoving = false;
    }
}
