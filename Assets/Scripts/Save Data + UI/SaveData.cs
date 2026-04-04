

using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    [System.Serializable]
    public class GameData
    {
        public string latestLevel;
        public bool mainGameCompleted;
        public bool unlockHardcore, hardcoreCompleted;
        public bool unlockChallengeZone, challengeZoneCompleted;
        public bool unlockGauntlets, gauntletsCompleted;
        public bool unlockTheTrials, theTrialsCompleted;
        public bool unlockEpilogue, epilogueCompleted;
        public bool unlockSuperHardcore, superHardcoreCompleted;
        public bool unlockEndGame, endGameCompleted;
    }

    public GameData data = new GameData();

    private void Start()
    {
        LoadGameData();

        if (SceneManager.GetActiveScene().name == "Main Menu") // Make sure to replace this with your actual Main Menu scene name
        {
            UpdateMainMenuUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu"); // Make sure to replace this with your actual Main Menu scene name
        }
    }

    public void SaveGameData()
    {
        // Save to PlayerPrefs
        PlayerPrefs.SetString("LatestLevel", SceneManager.GetActiveScene().name);
        Debug.Log(data.latestLevel);
        PlayerPrefs.SetInt("MainGameCompleted", data.mainGameCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockHardcore", data.unlockHardcore ? 1 : 0);
        PlayerPrefs.SetInt("HardcoreCompleted", data.hardcoreCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockChallengeZone", data.unlockChallengeZone ? 1 : 0);
        PlayerPrefs.SetInt("ChallengeZoneCompleted", data.challengeZoneCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockGauntlets", data.unlockGauntlets ? 1 : 0);
        PlayerPrefs.SetInt("GauntletsCompleted", data.gauntletsCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockTheTrials", data.unlockTheTrials ? 1 : 0);
        PlayerPrefs.SetInt("TheTrialsCompleted", data.theTrialsCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockEpilogue", data.unlockEpilogue ? 1 : 0);
        PlayerPrefs.SetInt("EpilogueCompleted", data.epilogueCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockSuperHardcore", data.unlockSuperHardcore ? 1 : 0);
        PlayerPrefs.SetInt("SuperHardcoreCompleted", data.superHardcoreCompleted ? 1 : 0);
        PlayerPrefs.SetInt("UnlockEndGame", data.unlockEndGame ? 1 : 0);
        PlayerPrefs.SetInt("EndGameCompleted", data.endGameCompleted ? 1 : 0);
        PlayerPrefs.Save();
        // Save to JSON file
        //string json = JsonUtility.ToJson(data);
        //File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGameData()
    {
        // Load from PlayerPrefs
        data.latestLevel = PlayerPrefs.GetString("LatestLevel", SceneManager.GetActiveScene().name);
        data.mainGameCompleted = PlayerPrefs.GetInt("MainGameCompleted", 0) == 1;
        data.unlockHardcore = PlayerPrefs.GetInt("UnlockHardcore", 0) == 1;
        data.hardcoreCompleted = PlayerPrefs.GetInt("HardcoreCompleted", 0) == 1;
        data.unlockChallengeZone = PlayerPrefs.GetInt("UnlockChallengeZone", 0) == 1;
        data.challengeZoneCompleted = PlayerPrefs.GetInt("ChallengeZoneCompleted", 0) == 1;
        data.unlockGauntlets = PlayerPrefs.GetInt("UnlockGauntlets", 0) == 1;
        data.gauntletsCompleted = PlayerPrefs.GetInt("GauntletsCompleted", 0) == 1;
        data.unlockTheTrials = PlayerPrefs.GetInt("UnlockTheTrials", 0) == 1;
        data.theTrialsCompleted = PlayerPrefs.GetInt("TheTrialsCompleted", 0) == 1;
        data.unlockEpilogue = PlayerPrefs.GetInt("UnlockEpilogue", 0) == 1;
        data.epilogueCompleted = PlayerPrefs.GetInt("EpilogueCompleted", 0) == 1;
        data.unlockSuperHardcore = PlayerPrefs.GetInt("UnlockSuperHardcore", 0) == 1;
        data.superHardcoreCompleted = PlayerPrefs.GetInt("SuperHardcoreCompleted", 0) == 1;
        data.unlockEndGame = PlayerPrefs.GetInt("UnlockEndGame", 0) == 1;
        data.endGameCompleted = PlayerPrefs.GetInt("EndGameCompleted", 0) == 1;

        // Load from JSON file
        //string path = Application.persistentDataPath + "/savefile.json";
        //if (File.Exists(path))
        //{
        //    string json = File.ReadAllText(path);
        //    data = JsonUtility.FromJson<GameData>(json);
        //}
    }

    private void UpdateMainMenuUI()
    {
        UpdateButtonState("MainGameButton", true, data.mainGameCompleted);
        UpdateButtonState("HardcoreButton", data.unlockHardcore, data.hardcoreCompleted);
        UpdateButtonState("ChallengeZoneButton", data.unlockChallengeZone, data.challengeZoneCompleted);
        UpdateButtonState("GauntletsButton", data.unlockGauntlets, data.gauntletsCompleted);
        UpdateButtonState("TheTrialsButton", data.unlockTheTrials, data.theTrialsCompleted);
        UpdateButtonState("EpilogueButton", data.unlockEpilogue, data.epilogueCompleted);
        UpdateButtonState("SuperHardcoreButton", data.unlockSuperHardcore, data.superHardcoreCompleted);
        UpdateButtonState("EndGameButton", data.unlockEndGame, data.endGameCompleted);

        void UpdateButtonState(string buttonTag, bool unlocked, bool completed)
        {
            Button button = GameObject.FindGameObjectWithTag(buttonTag)?.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = unlocked;
                ColorBlock cb = button.colors;
                cb.normalColor = completed ? Color.green : unlocked ? new Color(87, 88, 87) : Color.gray; // Adjust RGB values for dark teal
                button.colors = cb;
            }
        }
    }

public void OnSaveTrigger(
    string levelName, 
    bool mainGameCompleted, 
    bool unlockHardcore, 
    bool hardcoreCompleted, 
    bool unlockChallengeZone, 
    bool challengeZoneCompleted, 
    bool unlockGauntlets, 
    bool gauntletsCompleted, 
    bool unlockTheTrials, 
    bool theTrialsCompleted, 
    bool unlockEpilogue, 
    bool epilogueCompleted, 
    bool unlockSuperHardcore, 
    bool superHardcoreCompleted, 
    bool unlockEndGame, 
    bool endGameCompleted) 
{
    data.latestLevel = levelName;
    data.mainGameCompleted = mainGameCompleted;
    data.unlockHardcore = unlockHardcore;
    data.hardcoreCompleted = hardcoreCompleted;
    data.unlockChallengeZone = unlockChallengeZone;
    data.challengeZoneCompleted = challengeZoneCompleted;
    data.unlockGauntlets = unlockGauntlets;
    data.gauntletsCompleted = gauntletsCompleted;
    data.unlockTheTrials = unlockTheTrials;
    data.theTrialsCompleted = theTrialsCompleted;
    data.unlockEpilogue = unlockEpilogue;
    data.epilogueCompleted = epilogueCompleted;
    data.unlockSuperHardcore = unlockSuperHardcore;
    data.superHardcoreCompleted = superHardcoreCompleted;
    data.unlockEndGame = unlockEndGame;
    data.endGameCompleted = endGameCompleted;

    SaveGameData();
}

}
