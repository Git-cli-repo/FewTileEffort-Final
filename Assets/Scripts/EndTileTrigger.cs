using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro namespace

public class EndTileTrigger : MonoBehaviour
{
    // Arrays for different tower levels
    public string[] towerLevelsTo1 = { "To1-1", "To1-2", "To1-3", "To1-4", "To1-5" };
    public string[] towerLevelsTo2 = { "To2-1", "To2-2", "To2-3", "To2-4", "To2-5" };
    public string[] towerLevelsTo3 = { "To3-1", "To3-2", "To3-3", "To3-4", "To3-5" };
    public string[] towerLevelsTo4 = { "To4-1", "To4-2", "To4-3", "To4-4", "To4-5" };
    public string[] towerLevelsTo5 = { "To5-1", "To5-2", "To5-3", "To5-4", "To5-5" };

    // Reference to the TextMeshPro text object
    public TextMeshProUGUI currentHeightText;

    // Maximum height per level range
    public int heightThreshold = 20;

    // Trigger function when player hits the end tile
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerObj")) // Assuming your player is tagged with "PlayerObj"
        {
            IncrementHeightAndLoadLevel();
        }
    }

    // Function to increment height and load a random scene based on the current height
    void IncrementHeightAndLoadLevel()
    {
        // Get current height from PlayerPrefs (default is 0)
        int currentHeight = PlayerPrefs.GetInt("CurrentHeight", 0);

        // Increment the height
        currentHeight++;

        // Save the new height back to PlayerPrefs
        PlayerPrefs.SetInt("CurrentHeight", currentHeight);

        // Update the TextMeshPro object with the current height
        UpdateHeightText(currentHeight);

        // Load the correct set of levels based on the current height
        if (currentHeight > 80)
        {
            LoadRandomLevel(towerLevelsTo5);
        }
        else if (currentHeight > 60)
        {
            LoadRandomLevel(towerLevelsTo4);
        }
        else if (currentHeight > 40)
        {
            LoadRandomLevel(towerLevelsTo3);
        }
        else if (currentHeight > 20)
        {
            LoadRandomLevel(towerLevelsTo2);
        }
        else
        {
            LoadRandomLevel(towerLevelsTo1);
        }
    }

    // Function to load a random level from a given array of level names
    void LoadRandomLevel(string[] levelArray)
    {
        int randomIndex = Random.Range(0, levelArray.Length);
        SceneManager.LoadScene(levelArray[randomIndex]);
    }

    // Clear the current height and load a new random tower level
    public void ClearAndLoadRandomTowerLevel()
    {
        PlayerPrefs.SetInt("CurrentHeight", 0); // Reset height
        UpdateHeightText(0); // Update text to reflect reset
        LoadRandomLevel(towerLevelsTo1); // Start from To1
    }

    // Function to update the TextMeshPro object with the current height
    void UpdateHeightText(int height)
    {
        if (currentHeightText != null)
        {
            currentHeightText.text = "Current Height: " + height.ToString();
        }
    }
}
