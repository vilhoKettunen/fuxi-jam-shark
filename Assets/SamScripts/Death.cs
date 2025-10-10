using UnityEngine;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    public AudioClip deathSound;          // Sound to play when player dies
    public GameObject deathEffectPrefab;  // Prefab to spawn at death location
    public float respawnDelay = 5f;       // Time before player respawns

    private bool isDead = false;
public bool IsDead => isDead;

    public void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("dmg"))
        {
            StartCoroutine(HandleDeath());
        }
    }

    public IEnumerator HandleDeath()
    {
        isDead = true;

        // Spawn effect
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Play death sound (independent of player object)
        if (deathSound != null)
        {
            GameObject soundObj = new GameObject("DeathSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = deathSound;
            source.Play();
            Destroy(soundObj, deathSound.length);
        }

        // Disable movement to prevent errors
        var moveScript = GetComponent<PlayerController>();
        if (moveScript != null)
            moveScript.enabled = false;

        // Disable visuals and collisions
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        // Wait before respawning
        yield return new WaitForSeconds(respawnDelay);

        // Respawn at (0, 0, 0)
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
