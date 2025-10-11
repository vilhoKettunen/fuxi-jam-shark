using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Tooltip("Tag of the player object to affect")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerDeathHandler deathHandler = other.GetComponent<PlayerDeathHandler>();
            if (deathHandler != null)
            {
                // Start the player's death sequence
                StartCoroutine(deathHandler.HandleDeath());
            }
            else
            {
                Debug.LogWarning("Player does not have a PlayerDeathHandler component!");
            }
        }
    }
}
