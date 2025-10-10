using UnityEngine;
using UnityEngine.UI;

public class PlayerProgressBar : MonoBehaviour
{
    [Header("References")]
    public Transform player;         // Player's transform
    public Slider progressBar;       // The UI slider for progress

    [Header("Level Boundaries (X Axis)")]
    public float startX = 0f;        // Starting X position
    public float endX = 100f;        // Ending X position

    private void Update()
    {
        if (player == null || progressBar == null) return;

        // Get player's current X position
        float playerX = player.position.x;

        // Normalize progress between 0 and 1
        float progress = Mathf.InverseLerp(startX, endX, playerX);

        // Clamp it between 0 and 1
        progress = Mathf.Clamp01(progress);

        // Update slider
        progressBar.value = progress;
    }
}