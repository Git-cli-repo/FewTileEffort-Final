using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.CSharp;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using Steamworks;
// ----------------------------
// Data classes for saving/loading states
// ----------------------------
[System.Serializable]
public class AchievementRecord
{
    public string achievementID;
    public bool isUnlocked;
}

[System.Serializable]
public class AchievementsSaveData
{
    public List<AchievementRecord> records = new List<AchievementRecord>();
}

// ----------------------------
// Runtime container for each achievement's metadata
// ----------------------------
[System.Serializable]
public class AchievementInfo
{
    public string achievementID;
    public string title;
    public string description;
}

// ----------------------------
// The main AchievementManager
// ----------------------------
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("File Config")]
    [Tooltip("Name of the JSON file to store achievement states.")]
    public string achievementsFileName;

    // List of all achievements, auto-populated in code.
    [SerializeField]
    private List<AchievementInfo> allAchievements = new List<AchievementInfo>();

    // Dictionary to track unlocked states
    private Dictionary<string, bool> unlockedStates = new Dictionary<string, bool>();

    // Data structure for saving/loading
    private AchievementsSaveData saveData;

    void Awake()
    {
        // Basic Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        Debug.LogWarning("ENTERED AchievementManager.Start()");
        if (AchievementManager.Instance.ReadFromFile("tracker.json", "currentSaveFileName", true) == null)
        {
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson("tracker.json", new Dictionary<string, string>(), true);
            achievementsFileName = AchievementManager.Instance.ReadFromFile("tracker.json", "currentSaveFileName", true);
            Debug.LogWarning($"Set Achievement File Name To: {AchievementManager.Instance.ReadFromFile("tracker.json", "currentSaveFileName", true)}");
            Debug.LogWarning($"Achievement File Name Is Currently: {achievementsFileName}");
            Debug.LogError(Application.persistentDataPath);

        }
        else
        {
            achievementsFileName = AchievementManager.Instance.ReadFromFile("tracker.json", "currentSaveFileName", true);
        }

        // 1) Populate all achievements in code
        PopulateDefaultAchievements();

        // 2) Load from file to restore any unlocked states
        LoadAchievementsFromFile();   
    }

    // ----------------------------------------------------
    // Hard-code all 29 achievements here
    // ----------------------------------------------------
    private void PopulateDefaultAchievements()
    {
        Debug.LogWarning("ENTERED PopulateDefaultAchievements()");

        allAchievements.Clear();

        // Original 14
        // 1) We All Start Somewhere
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "WE_ALL_START_SOMEWHERE",
            title = "We All Start Somewhere",
            description = "Complete the Main Game"
        });

        // 2) Bittersweet
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "BITTERSWEET",
            title = "Bittersweet",
            description = "Complete the Epilogue"
        });

        // 3) Timings Galore
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "TIMINGS_GALORE",
            title = "Timings Galore",
            description = "Complete Hardcore Mode"
        });

        // 4) Nowhere but Up
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "NOWHERE_BUT_UP",
            title = "Nowhere but Up",
            description = "Complete The Trials"
        });

        // 5) New Features?
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "NEW_FEATURES",
            title = "New Features?",
            description = "Complete The Gauntlets"
        });

        // 6) Getting Somewhere
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "GETTING_SOMEWHERE",
            title = "Getting Somewhere",
            description = "Complete the Challenge Zone"
        });

        // 7) Start of Hard
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "START_OF_HARD",
            title = "Start of Hard",
            description = "Complete Completionist Main Game"
        });

        // 8) Not Again...
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "NOT_AGAIN",
            title = "Not Again...",
            description = "Complete Completionist Epilogue"
        });

        // 9) Too Hard 4 U
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "TOO_HARD_4_U",
            title = "Too Hard 4 U",
            description = "Complete Completionist Challenge Zone"
        });

        // 10) Speed Demon
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "SPEED_DEMON",
            title = "Speed Demon",
            description = "Complete the Main Game in under 10 minutes"
        });

        // 11) Gotta Go Fast
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "GOTTA_GO_FAST",
            title = "Gotta Go Fast",
            description = "Complete Hardcore Mode in under 10 minutes"
        });

        // 12) Going Into Overdrive
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "GOING_INTO_OVERDRIVE",
            title = "Going Into Overdrive",
            description = "Complete Completionist Hardcore in under 10 minutes"
        });

        // 13) Expert++
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "EXPERT_PLUS_PLUS",
            title = "Expert++",
            description = "Complete Completionist Hardcore"
        });

        // 14) The True Ending
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "THE_TRUE_ENDING",
            title = "The True Ending",
            description = "Complete Completionist All-In-One"
        });

        // ----------------------------------------------------
        // 6 NEW ONES (to reach 20)
        // ----------------------------------------------------

        // 15) I'm Totally Dashless
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "TOTALLY_DASHLESS",
            title = "I'm Totally Dashless",
            description = "Complete the Main Game, Epilogue, and Hardcore without dashing"
        });

        // 16) To a Galaxy Far Away
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "GALAXY_FAR_AWAY",
            title = "To a Galaxy Far Away",
            description = "Bounce so high above the level bounds"
        });

        // 17) Grapple Master
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "GRAPPLE_MASTER",
            title = "Grapple Master",
            description = "Perform 10 successful grapples in a single level"
        });

        // 18) Master Detective
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "MASTER_DETECTIVE",
            title = "Master Detective",
            description = "Find The Secret hidden somewhere in The Tower"
        });

        // 19) Double Time
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "DOUBLE_TIME",
            title = "Double Time",
            description = "Complete the Main Game in under 5 minutes"
        });

        // 20) Tower Conqueror
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "TOWER_CONQUEROR",
            title = "Tower Conqueror",
            description = "Complete all Tower Worlds"
        });

        // 21) At Least You Tried
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "AT_LEAST_YOU_TRIED",
            title = "At Least You Tried",
            description = "Die 1,000 Times"
        });

        // 22) Ok, You Just Suck
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "OK_YOU_JUST_SUCK",
            title = "Ok, You Just Suck",
            description = "Die 10,000 Times"
        });

        // 23) You TRIED to Get This
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "YOU_TRIED_TO_GET_THIS",
            title = "You TRIED to Get This",
            description = "Die 50,000 Times"
        });

        // 24) The Dashless Express
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "THE_DASHLESS_EXPRESS",
            title = "The Dashless Express",
            description = "Beat Completionist Main Game, Epilogue, and Hardcore without dashing"
        });

        // 25) Super Dasher
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "SUPER_DASHER",
            title = "Super Dasher",
            description = "Dash 500 times in a single run"
        });

        // 26) Ascention to New Heights
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "ASCENTION_TO_NEW_HEIGHTS",
            title = "Ascention to New Heights",
            description = "Beat the Completionist Tower"
        });

        // 27) King of Pain
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "KING_OF_PAIN",
            title = "King of Pain",
            description = "Beat the Rooms"
        });

        // 28) Almost There!
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "ALMOST_THERE",
            title = "Almost There!",
            description = "Beat the Completionist Rooms"
        });

        // 29) Almost There!
        allAchievements.Add(new AchievementInfo
        {
            achievementID = "NEW_FEATURES_AGAIN",
            title = "New Features.... Again?",
            description = "Beat the Completionist Gauntlets"
        });
        Debug.LogWarning($"EXITED PopulateDefaultAchievements(). allAchievements has a count of: {allAchievements.Count}");
    }

    // ----------------------------------------------------
    // Public API for checking/unlocking achievements
    // ----------------------------------------------------
    public bool IsUnlocked(string achievementID)
    {
        return unlockedStates.ContainsKey(achievementID) && unlockedStates[achievementID];
    }

    public void UnlockAchievement(string achievementID)
    {
        if (!IsUnlocked(achievementID))
        {
            unlockedStates[achievementID] = true;
            Debug.Log("Achievement Unlocked: " + achievementID);
            SaveAchievementsToFile();
        }

        bool success = SteamUserStats.SetAchievement("WE_ALL_START_SOMEWHERE");
        bool stored = SteamUserStats.StoreStats();
        
        Debug.Log($"[Attempt] Achievement: {success}, Stored: {stored}");

        if (!success)
        {
            // If this is STILL false, Steam literally doesn't see the name
            Debug.LogError("Steam does not recognize the Achievement API Name. Check if Published!");
        }
    }

    // ----------------------------------------------------
    // Load from JSON file
    // ----------------------------------------------------
    public void LoadAchievementsFromFile()
    {
        Debug.LogWarning("ENTERED LoadAchievementsFromFile()");
        string filePath = Path.Combine(Application.persistentDataPath, achievementsFileName);

        if (File.Exists(filePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
        {
            string json = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<AchievementsSaveData>(json);

            // Build dictionary
            unlockedStates.Clear();
            foreach (var record in saveData.records)
            {
                unlockedStates[record.achievementID] = record.isUnlocked;
            }

            Debug.Log("Loaded achievements from: " + filePath);
        }
        else
        {
            // No file => new data
            saveData = new AchievementsSaveData();
            unlockedStates.Clear();

            SaveAchievementsToFile();

            Debug.Log("No achievements file found. Creating new data set.");
        }
        Debug.LogWarning($"EXITED LoadAchievementsFromFile(). filePath={filePath}, unlockedStates contains {unlockedStates.Count}, actually set to unlocked are {unlockedStates.Where(p => p.Value == true).ToList().Count}");
    }

    // ----------------------------------------------------
    // Save to JSON file
    // ----------------------------------------------------
    private void SaveAchievementsToFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, achievementsFileName);

        // Rebuild from dictionary
        saveData.records.Clear();
        foreach (var kvp in unlockedStates)
        {
            AchievementRecord record = new AchievementRecord
            {
                achievementID = kvp.Key,
                isUnlocked = kvp.Value
            };
            saveData.records.Add(record);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Achievements saved to: " + filePath + " at count " + saveData.records.Count);
    }


    public void WriteToFile(string fileName, int toWrite, string propToEdit)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string fileCurrentContents = File.ReadAllText(filePath);
        dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
        fileJson[propToEdit] = toWrite;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fileJson, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved content " + toWrite + " from " + propToEdit + " to " + filePath);
    }

    public void WriteToFile(string fileName, float toWrite, string propToEdit)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string fileCurrentContents = File.ReadAllText(filePath);
        dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
        fileJson[propToEdit] = toWrite;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fileJson, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved content " + toWrite + " from " + propToEdit + " to " + filePath);
    }

    public int ReadFromFile(string fileName, string propToRead)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File does not exist, resetting {fileName}.");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"int", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead);

        }

        string fileCurrentContents = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(fileCurrentContents))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File is empty, resetting {fileName}");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"int", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead);
        }

        if (File.Exists(filePath))
        {
            dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
            int readFromFile = fileJson[propToRead];
            Debug.Log("Read content " + readFromFile + " in " + propToRead + " to " + filePath);
            return readFromFile;
        }
        else
        {
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"int", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead);

        }

    }

    public float ReadFromFile(string fileName, string propToRead, float randFloat)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File does not exist, resetting {fileName}");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"float", propToRead}
            }, false);
            return ReadFromFile("tracker.json", propToRead);

        }

        string fileCurrentContents = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(fileCurrentContents))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File is empty, resetting {fileName}");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"float", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead);
        }

        if (File.Exists(filePath))
        {
            dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
            int readFromFile = fileJson[propToRead];
            Debug.Log("Read content " + readFromFile + " in " + propToRead + " to " + filePath);
            return readFromFile;
        }
        else
        {
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"int", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead);

        }

    }



    public string ReadFromFile(string fileName, string propToRead, bool randoBool)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File does not exist, resetting {fileName}");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"string", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead, true);

        }

        string fileCurrentContents = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(fileCurrentContents))
        {
            Debug.Log("Retrying loop....");
            Debug.Log($"File is empty, resetting {fileName}");
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"string", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead, true);
        }

        if (File.Exists(filePath))
        {
            dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
            string readFromFile = fileJson[propToRead];
            return readFromFile;
        }
        else
        {
            CompleteAchievementsRunManager.Instance.ResetTrackerDotJson(fileName, new Dictionary<string, string>
            {
                {"string", propToRead}
            }, false);
            return ReadFromFile(fileName, propToRead, true);

        }

    }

    public void WriteToFile(string fileName, string toWrite, string propToEdit)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "tracker.json");
        string fileCurrentContents = File.ReadAllText(filePath);
        dynamic fileJson = Newtonsoft.Json.JsonConvert.DeserializeObject(fileCurrentContents);
        fileJson[propToEdit] = toWrite;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fileJson, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);

        Debug.Log("Saved content to " + filePath);

    }

    // ----------------------------------------------------
    // (Optional) Access for a UI manager to list achievements
    // ----------------------------------------------------
    public List<AchievementInfo> GetAllAchievements()
    {
        if (allAchievements.Count == 0)
        {
            PopulateDefaultAchievements();
            LoadAchievementsFromFile();
        }
        return allAchievements;
    }

    public void CreateNewSaveFile(string nameOfSaveFile, bool copySaveFile, string saveFileToCopy)
    {
        if (copySaveFile)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, nameOfSaveFile), File.ReadAllText(Path.Combine(Application.persistentDataPath, achievementsFileName)));
            AchievementManager.Instance.achievementsFileName = nameOfSaveFile;
            AchievementManager.Instance.WriteToFile("tracker.json", nameOfSaveFile, "currentSaveFileName");
            AchievementManager.Instance.WriteToFile("tracker.json", AchievementManager.Instance.ReadFromFile("tracker.json", "allApplicableSaveFiles") + "," + nameOfSaveFile, "allApplicableSaveFiles");
            LoadAchievementsFromFile();
        }
        else
        {
            AchievementManager.Instance.achievementsFileName = nameOfSaveFile;
            AchievementManager.Instance.WriteToFile("tracker.json", nameOfSaveFile, "currentSaveFileName");
            AchievementManager.Instance.WriteToFile("tracker.json", AchievementManager.Instance.ReadFromFile("tracker.json", "allApplicableSaveFiles") + "," + nameOfSaveFile, "allApplicableSaveFiles");
            LoadAchievementsFromFile();
        }
    }

    public void SetSaveFile(string nameOfSaveFile)
    {
        if (AchievementManager.Instance.ReadFromFile("tracker.json", "allApplicableSaveFiles").ToString().Split(",").ToList().Where(p => !string.IsNullOrEmpty(p) && !string.IsNullOrWhiteSpace(p)).Contains(nameOfSaveFile))
        {
            AchievementManager.Instance.achievementsFileName = nameOfSaveFile;
            AchievementManager.Instance.WriteToFile("tracker.json", nameOfSaveFile, "currentSaveFileName");
            AchievementManager.Instance.WriteToFile("tracker.json", AchievementManager.Instance.ReadFromFile("tracker.json", "allApplicableSaveFiles") + "," + nameOfSaveFile, "allApplicableSaveFiles");
            LoadAchievementsFromFile();
        }
        else
        {
            CreateNewSaveFile(nameOfSaveFile, false, "__PLACEHOLDER__");
        }
    }
}
