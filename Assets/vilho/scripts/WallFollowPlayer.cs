using UnityEngine;
using System.Collections;

public class WallFollowPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement Settings")]
    [Tooltip("How fast the wall moves toward the player.")]
    public float speed = 5f;

    [Tooltip("Maximum distance the wall can be from the player.")]
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

        // ✅ If wall ever passes the player on the X axis, reset it immediately
        if (transform.position.x > player.position.x)
        {
            Debug.Log("⚠️ Wall moved ahead of the player — resetting behind.");
            ResetWallPosition();
            return; // stop further movement this frame
        }

        // Move toward the player along the X axis
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // Clamp to max distance
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
            // Ensure wall reference is linked for death resets
            deathHandler.wall = this;
            StartCoroutine(deathHandler.HandleDeath());
        }
        else
        {
            Debug.LogWarning("⚠️ Player does not have a PlayerDeathHandler component!");
        }

        // Start respawn delay
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
    /// Resets the wall behind the player or at the designated reset point.
    /// </summary>
    public void ResetWallPosition()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        isWaitingToRespawn = false;

        if (player == null)
        {
            Debug.LogError("❌ No player assigned for wall reset.");
            return;
        }

        Vector3 newPos;

        // If a reset point is assigned, use that.
        if (resetPoint != null)
        {
            newPos = resetPoint.position;
            Debug.Log($"🧱 Wall reset to designated reset point: {newPos}");
        }
        else
        {
            // Otherwise, always spawn to the left of the player
            newPos = new Vector3(
                player.position.x - spawnDistanceFromPlayer,
                transform.position.y,
                transform.position.z
            );
            Debug.Log($"↩️ Wall reset {spawnDistanceFromPlayer} units behind player at X={newPos.x}");
        }

        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Max distance (red)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, maxDistance);

            // Spawn offset (yellow)
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
