using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MissileSeek : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;
    public float speed = 10f;
    public float rotateSpeed = 3f;
    public float lifetime = 10f;

    [Header("Accuracy")]
    [Range(0f, 5f)] public float aimDrift = 0.5f;
    [Range(0f, 1f)] public float turnResponsiveness = 0.8f;

    [Header("Audio & Effects")]
    public AudioClip flightSound;
    public AudioClip impactSound;
    public GameObject explosionPrefab;
    public float explosionLifetime = 2f;

    private AudioSource audioSource;
    private bool hasExploded = false;
    private float fixedZ;
    private Vector3 driftOffset;

    void Start()
    {
        fixedZ = transform.position.z;

        driftOffset = new Vector3(
            Random.Range(-aimDrift, aimDrift),
            Random.Range(-aimDrift, aimDrift),
            0f // Z locked
        );

        audioSource = GetComponent<AudioSource>();
        if (flightSound != null)
        {
            audioSource.clip = flightSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasExploded || target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, fixedZ) + driftOffset;
        Vector3 direction = (targetPos - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            rotateSpeed * turnResponsiveness * Time.deltaTime
        );

        transform.position += transform.forward * speed * Time.deltaTime;

        // Lock Z axis
        transform.position = new Vector3(transform.position.x, transform.position.y, fixedZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        // Explode if it's anything other than the player
        if (!other.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Stop flight sound
        if (audioSource != null)
            audioSource.Stop();

        // Play impact sound
        if (impactSound != null)
            AudioSource.PlayClipAtPoint(impactSound, transform.position);

        // Spawn explosion prefab
        if (explosionPrefab != null)
        {
            GameObject ex = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(ex, explosionLifetime);
        }

        // Destroy missile immediately
        Destroy(gameObject);
    }
}
