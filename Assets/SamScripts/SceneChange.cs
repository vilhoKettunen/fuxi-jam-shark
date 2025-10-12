using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderOnPlayerCollision : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The name of the scene to load when the player collides with this object.")]
    public string sceneToLoad = "NextLevel";

    [Header("Settings")]
    [Tooltip("If true, waits a short delay before loading the scene.")]
    public float loadDelay = 0f;

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isLoading && other.CompareTag("Player"))
        {
            isLoading = true;

            if (loadDelay > 0)
                Invoke(nameof(LoadScene), loadDelay);
            else
                LoadScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLoading && collision.collider.CompareTag("Player"))
        {
            isLoading = true;

            if (loadDelay > 0)
                Invoke(nameof(LoadScene), loadDelay);
            else
                LoadScene();
        }
    }

    private void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneLoaderOnPlayerCollision: No scene name specified!");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
