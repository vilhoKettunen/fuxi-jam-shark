using UnityEngine;

public class DashPowerup : MonoBehaviour
{
    [Header("Hover Settings")]
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 2f;
    public float rotationSpeed = 90f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Hover motion (sin wave)
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Rotate in place
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.UnlockDash();
            Destroy(gameObject);
        }
    }
}