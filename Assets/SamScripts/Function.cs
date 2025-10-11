using UnityEngine;

public class PrefabTouchCounter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the prefab that should be counted when touched.")]
    public string targetPrefabTag = "Collectible";

    [Tooltip("Tag of the objects to remove after reaching the goal.")]
    public string objectsToRemoveTag = "Obstacle";

    [Tooltip("How many prefabs must be touched before removing objects.")]
    public int targetCount = 3;

    private int currentCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object the player touched has the right tag
        if (other.CompareTag(targetPrefabTag))
        {
            // Increase counter
            currentCount++;

            // Destroy the prefab that was touched
            Destroy(other.gameObject);

            // Check if target count reached
            if (currentCount >= targetCount)
            {
                RemoveObjectsWithTag(objectsToRemoveTag);
            }
        }
    }

    private void RemoveObjectsWithTag(string tag)
    {
        GameObject[] objectsToRemove = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objectsToRemove)
        {
            Destroy(obj);
        }

        Debug.Log($"Removed {objectsToRemove.Length} objects with tag '{tag}'.");
    }
}

