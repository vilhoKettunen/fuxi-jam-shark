using UnityEngine;
using System.Collections;

public class WallFollowPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement Settings")]
    [Tooltip("How fast the wall moves toward the player.")]
    public float speed = 5f;

    [Tooltip("Minimum distance the wall maintains from the player.")]
    public float maxDistance = 10f;

    [Header("Spawn & Reset Settings")]
    [Tooltip("How far to spawn/reset the wall on the left side of the player (negative X direction).")]
    public float spawnDistanceFromPlayer = 20f;

    [Tooltip("Optional fixed reset point. If assigned, wall always teleports here on player death.")]
    public Transform resetPoint;

    [Header("Collision & Respawn Settings")]
    public float respawnDelay = 3f;

    [Header("Player Settings")]
    public string playerTag = "Player";

    private bool isWaitingToRespawn = false;
    private Coroutine respawnCoroutine;

    private void Update()
    {
        if (player == null || isWaitingToRespawn) return;

        // Move the wall toward the player along X only
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // Clamp max distance so the wall doesn't get too far away
        float currentDistance = Mathf.Abs(player.position.x - transform.position.x);
        if (currentDistance > maxDistance)
        {
            float clampedX = player.position.x - Mathf.Sign(player.position.x - transform.position.x) * maxDistance;
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag) || isWaitingToRespawn) return;

        Debug.Log("💀 Wall hit the player!");

        // Kill the player
        PlayerDeathHandler deathHandler = other.GetComponent<PlayerDeathHandler>();
        if (deathHandler != null)
        {
            // Link wall so player death can also reset it
            deathHandler.wall = this;
            StartCoroutine(deathHandler.HandleDeath());
        }
        else
        {
            Debug.LogWarning("⚠️ Player does not have a PlayerDeathHandler component!");
        }

        // Start wall respawn coroutine
        if (respawnCoroutine != null)
            StopCoroutine(respawnCoroutine);

        respawnCoroutine = StartCoroutine(HandleRespawnDelay());
    }

    private IEnumerator HandleRespawnDelay()
    {
        isWaitingToRespawn = true;
        yield return new WaitForSeconds(respawnDelay);
        ResetWallPosition();
        isWaitingToRespawn = false;
    }

    /// <summary>
    /// Teleports the wall to its reset position — always on the left side of the player or reset point.
    /// </summary>
    public void ResetWallPosition()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        isWaitingToRespawn = false;

        if (resetPoint != null)
        {
            // ✅ Use the reset point if defined
            transform.position = resetPoint.position;
            Debug.Log($"🧱 Wall reset to designated reset point: {resetPoint.position}");
        }
        else if (player != null)
        {
            // ✅ Otherwise spawn to the left (negative X) of the player
            Vector3 spawnPosition = new Vector3(
                player.position.x - spawnDistanceFromPlayer,
                transform.position.y,
                transform.position.z
            );
            transform.position = spawnPosition;
            Debug.Log($"🧱 Wall reset {spawnDistanceFromPlayer} units to the left of player at {spawnPosition}");
        }
        else
        {
            Debug.LogError("❌ No player or reset point assigned! Cannot reset wall.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Show max distance range (red)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, maxDistance);

            // Show spawn offset (yellow)
            Vector3 spawnPos = new Vector3(player.position.x - spawnDistanceFromPlayer, player.position.y, player.position.z);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
        }

        if (resetPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(resetPoint.position, 0.5f);
        }
    }
}