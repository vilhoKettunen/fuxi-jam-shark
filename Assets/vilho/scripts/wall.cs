using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public Transform player;       // Reference to the player's transform
    public float speed = 5f;       // Movement speed of the wall

    private void Update()
    {
        if (player == null) return;

        // Move toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    
}
