using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    string levelName;

    public void LoadLastCompletedLevel()
    {
        levelName = PlayerPrefs.GetString("LatestLevel", "DefaultLevelName");
        SceneManager.LoadScene(levelName);
    }
}
