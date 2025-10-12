using UnityEngine;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    public AudioClip deathSound;
    public GameObject deathEffectPrefab;
    public float respawnDelay = 5f;

    [Header("Wall Reset Settings")]
    [Tooltip("Reference to the wall that should reset when the player dies.")]
    public WallFollowPlayer wall; // Assign in Inspector

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        // Auto-find wall if not assigned
        if (wall == null)
        {
            wall = Object.FindFirstObjectByType<WallFollowPlayer>();
            if (wall != null)
                Debug.Log("✅ Wall automatically assigned to PlayerDeathHandler");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.CompareTag("dmg"))
        {
            StartCoroutine(HandleDeath());
        }
    }

    public IEnumerator HandleDeath()
    {
        // ✅ Teleport the wall to its reset position immediately
        if (wall != null)
        {
            wall.ResetWallPosition();
        }
        isDead = true;

        // Spawn death effect
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Play death sound
        if (deathSound != null)
        {
            GameObject soundObj = new GameObject("DeathSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = deathSound;
            source.Play();
            Destroy(soundObj, deathSound.length);
        }

        // Disable player movement
        var moveScript = GetComponent<PlayerController>();
        if (moveScript != null)
            moveScript.enabled = false;

        // Disable visuals and collisions
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

       

        // Wait before respawn
        yield return new WaitForSeconds(respawnDelay);

        // Respawn player
        transform.position = Vector3.zero;

        // Re-enable visuals and collisions
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = true;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        // Re-enable movement
        if (moveScript != null)
            moveScript.enabled = true;

        isDead = false;
    }
}
