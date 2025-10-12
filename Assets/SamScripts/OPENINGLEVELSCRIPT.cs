using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnter : MonoBehaviour
{
    [SerializeField] private string sceneName = "NextScene"; // Set your scene name in the Inspector

    void Update()
    {
        // Check if the Enter (Return) key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set!");
        }
    }
}
