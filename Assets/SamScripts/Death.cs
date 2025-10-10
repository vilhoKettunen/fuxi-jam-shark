using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Settings")]
    public AudioClip deathSound;          // Sound to play when player dies
    public GameObject deathEffectPrefab;  // Prefab to spawn at death location
    public float destroyDelay = 0.1f;     // Slight delay to allow sound to play

    private bool isDead = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("dmg"))
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Spawn effect
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Play death sound
        if (deathSound != null)
        {
            // Create a temporary AudioSource so sound plays even after player is destroyed
            GameObject soundObj = new GameObject("DeathSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = deathSound;
            source.Play();

            // Destroy the temp sound object when done
            Destroy(soundObj, deathSound.length);
        }

        // Disable player visuals & collisions
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        // Destroy player after short delay (so sound starts cleanly)
        gameObject.SetActive(false);
    }
}
