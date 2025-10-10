using UnityEngine;

public class Restart : MonoBehaviour
{
    [Tooltip("Reference to the PlayerDeathHandler script on the player")]
    public PlayerDeathHandler playerDeathHandler;

    void Start()
    {
        // Automatically find the player if not assigned in inspector
        if (playerDeathHandler == null)
        {
            playerDeathHandler = FindFirstObjectByType<PlayerDeathHandler>();
        }
    }

    void Update()
    {
        // Press R to manually trigger the player's death
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerDeathHandler == null)
            {
                Debug.LogWarning("No PlayerDeathHandler found in DamageTrigger!");
                return;
            }

            // Only trigger death if the player is currently alive
            if (!playerDeathHandler.IsDead)
            {
                playerDeathHandler.StartCoroutine(playerDeathHandler.HandleDeath());
            }
        }
    }
}
