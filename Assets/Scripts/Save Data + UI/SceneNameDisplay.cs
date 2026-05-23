using UnityEngine;
using UnityEngine.UI; // Include the UI namespace to work with Text components
using UnityEngine.SceneManagement; // Needed for scene management

public class SceneNameDisplay : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene loads

        // Update the text with the current scene name at startup
        UpdateTextWithSceneName(SceneManager.GetActiveScene().name);
        
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    // This method is called every time a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateTextWithSceneName(SceneManager.GetActiveScene().name);
    }

    // Updates the Text component with the given scene name
    private void UpdateTextWithSceneName(string sceneName)
    {
        Text textComponent = GetComponent<Text>(); // Get the Text component on this GameObject
        if (textComponent != null)
        {
            textComponent.text = "Scene: " + sceneName; // Update the text to show the scene name
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
