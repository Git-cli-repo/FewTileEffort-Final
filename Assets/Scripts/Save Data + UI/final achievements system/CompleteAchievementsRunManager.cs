using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Microsoft.SqlServer.Server;
using System.Data.Common;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Security.Policy;

public class CompleteAchievementsRunManager : MonoBehaviour
{
    public static CompleteAchievementsRunManager Instance;

    public enum GameMode
    {
        None,
        MainGame,
        MainGameComp,
        Epilogue,
        EpilogueComp,
        Hardcore,
        HardcoreComp,
        ChallengeZone,
        ChallengeZoneComp,
        Trials,
        Gauntlets,
        GauntletsComp,
        Tower,
        AllInOne,
        TowerComp,
        Rooms,
        RoomsComp,
        NotAMode
    }

    [Header("Scene Names for End Scenes")]
    public string mainGameEndScene = "MainGameEndScene";
    public string mainGameCompEndScene = "MainGameCompEndScene";
    public string epilogueEndScene = "EpilogueEndScene";
    public string epilogueCompEndScene = "EpilogueCompEndScene";
    public string hardcoreEndScene = "HardcoreEndScene";
    public string hardcoreCompEndScene = "HardcoreCompEndScene";
    public string challengeZoneEndScene = "ChallengeZoneEndScene";
    public string challengeZoneCompEndScene = "ChallengeZoneCompEndScene";
    public string trialsEndScene = "TrialsEndScene";
    public string gauntletsEndScene = "GauntletsEndScene";
    public string towerEndScene = "TowerEndScene";
    public string allInOneEndScene = "AllInOneEndScene";
    public string towerCompEndScene = "";
    public string roomsEndScene = "";
    public string roomsCompEndScene = "";
    public string gauntletsCompEndScene = "";

    [Header("Run State (Debug)")]
    public bool runActive = false;
    public GameMode currentMode = GameMode.None;
    private float runStartTime = 0f;

    // TOTALLY_DASHLESS logic: we only update these if user finished properly w/ no dash
    private bool mainGameDashlessDone = false;
    private bool epilogueDashlessDone = false;
    private bool hardcoreDashlessDone = false;

    // DASHLESS_EXPRESS logic: yada yada yada
    bool mainGameCompDashlessDone = false;
    bool epilogueCompDashlessDone = false;
    bool hardcoreCompDashlessDone = false;

    // We track dash usage for TOTALLY_DASHLESS
    private bool dashUsedThisRun = false;

    // Grapple Master
    private int grappleCountThisRun = 0;

    Queue<string> popupQueue = new Queue<string>();
    bool processingQueue = false;
    public GameObject popupPrefab;
    public Transform popupParent;

    // Galaxy Far Away
    private bool reachedHighBounce = false;
    [Header("Galaxy Far Away Threshold")]
    public float galaxyBounceThreshold = 500f;

    [Header("Death Related Numbers")]
    // Death Counts
    public int totalDeathCount = 0; // store globally, maybe load/save from achievements data
    public string currentPlayerColor = "FFFFFF";
    public int totalDashCount = 0;
    [Header("Level to Load in an testing emergency")]
    public string currentWorkingLevelToLoad = "To10-10";

    [Serializable]
    public class DeathData {
        public int deathCount;
    }
    public int timeToWait;
    public bool timerOff = true;

    public Color lockedColor;
    public List<GameObject> lockedButtons;
    public List<GameObject> unlockedButtons;

    [Header("IDs / First Levels")]
    public int mainGameID;
    public int mainGameCompID;
    public int epilogueID;
    public int epilogueCompID;
    public int hardcoreID;
    public int hardcoreCompID;
    public int challengeZoneID;
    public int challengeZoneCompID;
    public int trialsID;
    public int gauntletsID;
    public int towerID;
    public int allInOneID;
    public int towerCompID;
    public int roomsID;
    public int roomsCompID;
    public int gauntletsCompID;
    public TMP_Text deathText;

    [Header("Sound Related Things")]
    public string currentScene = "Main Menu";
    public bool isPlayingMusic;
    public List<string> mappingHelperSceneNames;
    public List<AudioClip> mappingHelperAudioClip;
    public AudioClip mainGame;
    public AudioClip epilogue;
    public AudioClip gauntlets;
    public AudioClip challengeZone;
    public AudioClip hardcore;
    public AudioClip tower;
    public AudioClip rooms;
    public AudioClip allInOne;
    public Dictionary<string, AudioClip> musReference = new Dictionary<string, AudioClip>();
    public AudioSource source;
    public TMP_Text saveFileText;
    public TMP_Text percentText;
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode switchTilesKey;
    public KeyCode enterPracticeModeKey;
    public KeyCode nextLevelKey;
    public KeyCode previousLevelKey;
    public KeyCode reloadKey;
    public KeyCode grappleKey;

    [Header("Mod Pack Related Items")]
    public bool isPlayingModPack = false;
    public bool isCompletionistModPack = false;
    public string activeModPackID = "";
    public int currentLevelIndex = 0;
    public int numberOfLevels = 0;
    public List<string> levelPaths = new List<string>();

    public List<string> completionistMainGame = new List<string>{
        // 0
        "C-W1L1",
        "C-W1L2",
        "C-W1L3",
        "C-W1L4",
        "C-W1L5",
        "C-W1L6",
        "C-W1L7",
        "C-W1L8",
        "C-W1L9",
        "C-W1L10",
        "C-W2L1",
        "C-W2L2",
        "C-W2L3",
        "C-W2L4",
        "C-W2L5",
        "C-W2L6",
        "C-W2L7",
        "C-W2L8",
        "C-W2L9",
        "C-W2L10",
        "C-W3L1",
        "C-W3L2",
        "C-W3L3",
        "C-W3L4",
        "C-W3L5",
        "C-W3L6",
        "C-W3L7",
        "C-W3L8",
        "C-W3L9",
        "C-W3L10"
    };
    public List<string> completionistEpilogue = new List<string>{
        // 1
        "C-Epilogue"
    };
    public List<string> completionistGauntlets = new List<string>{
        // 2
        "C-G1",
        "C-G2",
        "C-G3",
        "C-G4",
        "C-G5"
    };

    public List<string> completionistChallengeZone = new List<string> { 
        // 3
        "C-CZ-1",
        "C-CZ-2",
        "C-CZ-3",
        "C-CZ-4",
        "C-CZ-5",
        "C-CZ-6",
        "C-CZ-7",
        "C-CZ-8",
        "C-CZ-9",
        "C-CZ-10"
    };

    public List<string> completionistHardcore = new List<string>
    { 
        // 4
        "C-H1L1",
        "C-H1L2",
        "C-H1L3",
        "C-H1L4",
        "C-H1L5",
        "C-H1L6",
        "C-H1L7",
        "C-H1L8",
        "C-H1L9",
        "C-H1L10",
        "C-H2L1",
        "C-H2L2",
        "C-H2L3",
        "C-H2L4",
        "C-H2L5",
        "C-H2L6",
        "C-H2L7",
        "C-H2L8",
        "C-H2L9",
        "C-H2L10",
        "C-H3L1",
        "C-H3L2",
        "C-H3L3",
        "C-H3L4",
        "C-H3L5",
        "C-H3L6",
        "C-H3L7",
        "C-H3L8",
        "C-H3L9",
        "C-H3L10"
    };

    public List<string> completionistTower = new List<string>
    {
         // 5
        "C-To1-1",
        "C-To1-2",
        "C-To1-3",
        "C-To1-4",
        "C-To1-5",
        "C-To1-6",
        "C-To1-7",
        "C-To1-8",
        "C-To1-9",
        "C-To1-10",
        "C-To2-1",
        "C-To2-2",
        "C-To2-3",
        "C-To2-4",
        "C-To2-5",
        "C-To2-6",
        "C-To2-7",
        "C-To2-8",
        "C-To2-9",
        "C-To2-10",
        "C-To3-1",
        "C-To3-2",
        "C-To3-3",
        "C-To3-4",
        "C-To3-5",
        "C-To3-6",
        "C-To3-7",
        "C-To3-8",
        "C-To3-9",
        "C-To3-10",
        "C-To4-1",
        "C-To4-2",
        "C-To4-3",
        "C-To4-4",
        "C-To4-5",
        "C-To4-6",
        "C-To4-7",
        "C-To4-8",
        "C-To4-9",
        "C-To4-10",
        "C-To5-1",
        "C-To5-2",
        "C-To5-3",
        "C-To5-4",
        "C-To5-5",
        "C-To5-6",
        "C-To5-7",
        "C-To5-8",
        "C-To5-9",
        "C-To5-10",
        "C-To6-1",
        "C-To6-2",
        "C-To6-3",
        "C-To6-4",
        "C-To6-5",
        "C-To6-6",
        "C-To6-7",
        "C-To6-8",
        "C-To6-9",
        "C-To6-10",
        "C-To7-1",
        "C-To7-2",
        "C-To7-3",
        "C-To7-4",
        "C-To7-5",
        "C-To7-6",
        "C-To7-7",
        "C-To7-8",
        "C-To7-9",
        "C-To7-10",
        "C-To8-1",
        "C-To8-2",
        "C-To8-3",
        "C-To8-4",
        "C-To8-5",
        "C-To8-6",
        "C-To8-7",
        "C-To8-8",
        "C-To8-9",
        "C-To8-10",
        "C-To9-1",
        "C-To9-2",
        "C-To9-3",
        "C-To9-4",
        "C-To9-5",
        "C-To9-6",
        "C-To9-7",
        "C-To9-8",
        "C-To9-9",
        "C-To9-10",
        "C-To10-1",
        "C-To10-2",
        "C-To10-3",
        "C-To10-4",
        "C-To10-5",
        "C-To10-6",
        "C-To10-7",
        "C-To10-8",
        "C-To10-9",
        "C-To10-10"
    };

    public List<string> completionistRooms = new List<string>
    { 
        // 6
        "C-R-P1",
        "C-R-P2",
        "C-R-P3",
        "C-R-P4",
        "C-R-P5",
        "C-R-T1",
        "C-R-T2",
        "C-R-T3",
        "C-R-T4",
        "C-R-T5",
        "C-R-D1",
        "C-R-D2",
        "C-R-D3",
        "C-R-D4",
        "C-R-D5",
    };

    public List<string> completionistAllInOne = new List<string>
    { 
        // 7
        "AC-W1L1",
        "AC-W1L2",
        "AC-W1L3",
        "AC-W1L4",
        "AC-W1L5",
        "AC-W1L6",
        "AC-W1L7",
        "AC-W1L8",
        "AC-W1L9",
        "AC-W1L10",
        "AC-W2L1",
        "AC-W2L2",
        "AC-W2L3",
        "AC-W2L4",
        "AC-W2L5",
        "AC-W2L6",
        "AC-W2L7",
        "AC-W2L8",
        "AC-W2L9",
        "AC-W2L10",
        "AC-W3L1",
        "AC-W3L2",
        "AC-W3L3",
        "AC-W3L4",
        "AC-W3L5",
        "AC-W3L6",
        "AC-W3L7",
        "AC-W3L8",
        "AC-W3L9",
        "AC-W3L10",
        "AC-Epilogue",
        "AC-T1",
        "AC-G1",
        "AC-G2",
        "AC-G3",
        "AC-G4",
        "AC-G5",
        "AC-CZ-1",
        "AC-CZ-2",
        "AC-CZ-3",
        "AC-CZ-4",
        "AC-CZ-5",
        "AC-CZ-6",
        "AC-CZ-7",
        "AC-CZ-8",
        "AC-CZ-9",
        "AC-CZ-10",
        "AC-H1L1",
        "AC-H1L2",
        "AC-H1L3",
        "AC-H1L4",
        "AC-H1L5",
        "AC-H1L6",
        "AC-H1L7",
        "AC-H1L8",
        "AC-H1L9",
        "AC-H1L10",
        "AC-H2L1",
        "AC-H2L2",
        "AC-H2L3",
        "AC-H2L4",
        "AC-H2L5",
        "AC-H2L6",
        "AC-H2L7",
        "AC-H2L8",
        "AC-H2L9",
        "AC-H2L10",
        "AC-H3L1",
        "AC-H3L2",
        "AC-H3L3",
        "AC-H3L4",
        "AC-H3L5",
        "AC-H3L6",
        "AC-H3L7",
        "AC-H3L8",
        "AC-H3L9",
        "AC-H3L10",
        "AC-To1-1",
        "AC-To1-2",
        "AC-To1-3",
        "AC-To1-4",
        "AC-To1-5",
        "AC-To1-6",
        "AC-To1-7",
        "AC-To1-8",
        "AC-To1-9",
        "AC-To1-10",
        "AC-To2-1",
        "AC-To2-2",
        "AC-To2-3",
        "AC-To2-4",
        "AC-To2-5",
        "AC-To2-6",
        "AC-To2-7",
        "AC-To2-8",
        "AC-To2-9",
        "AC-To2-10",
        "AC-To3-1",
        "AC-To3-2",
        "AC-To3-3",
        "AC-To3-4",
        "AC-To3-5",
        "AC-To3-6",
        "AC-To3-7",
        "AC-To3-8",
        "AC-To3-9",
        "AC-To3-10",
        "AC-To4-1",
        "AC-To4-2",
        "AC-To4-3",
        "AC-To4-4",
        "AC-To4-5",
        "AC-To4-6",
        "AC-To4-7",
        "AC-To4-8",
        "AC-To4-9",
        "AC-To4-10",
        "AC-To5-1",
        "AC-To5-2",
        "AC-To5-3",
        "AC-To5-4",
        "AC-To5-5",
        "AC-To5-6",
        "AC-To5-7",
        "AC-To5-8",
        "AC-To5-9",
        "AC-To5-10",
        "AC-To6-1",
        "AC-To6-2",
        "AC-To6-3",
        "AC-To6-4",
        "AC-To6-5",
        "AC-To6-6",
        "AC-To6-7",
        "AC-To6-8",
        "AC-To6-9",
        "AC-To6-10",
        "AC-To7-1",
        "AC-To7-2",
        "AC-To7-3",
        "AC-To7-4",
        "AC-To7-5",
        "AC-To7-6",
        "AC-To7-7",
        "AC-To7-8",
        "AC-To7-9",
        "AC-To7-10",
        "AC-To8-1",
        "AC-To8-2",
        "AC-To8-3",
        "AC-To8-4",
        "AC-To8-5",
        "AC-To8-6",
        "AC-To8-7",
        "AC-To8-8",
        "AC-To8-9",
        "AC-To8-10",
        "AC-To9-1",
        "AC-To9-2",
        "AC-To9-3",
        "AC-To9-4",
        "AC-To9-5",
        "AC-To9-6",
        "AC-To9-7",
        "AC-To9-8",
        "AC-To9-9",
        "AC-To9-10",
        "AC-To10-1",
        "AC-To10-2",
        "AC-To10-3",
        "AC-To10-4",
        "AC-To10-5",
        "AC-To10-6",
        "AC-To10-7",
        "AC-To10-8",
        "AC-To10-9",
        "AC-To10-10"
    };

    public List<string> endSceneNames = new List<string>
    {
        "EndOfGame",
        "EndOfEpilogue",
        "EndOfHardcore",
        "EndOfChallengeZone",
        "EndOfGauntlets",
        "EndOfTrials",
        "To1-End",
        "To2-End",
        "To3-End",
        "To4-End",
        "To5-End",
        "To6-End",
        "To7-End",
        "To8-End",
        "To9-End",
        "To10-End",
        "EndOfPainRooms",
        "EndOfTortureRooms",
        "EndOfDeathRooms",
        "EndOfCMainGame",
        "EndOfCEpilogue",
        "EndOfCHardcore",
        "EndOfCChallengeZone",
        "EndOfCGauntlets",
        "EndOfTrialsC"
    };

    public List<KeyCode> keyCodes = new List<KeyCode>()
    {
        
    };

    public bool inPracticeMode = false;

    public List<string> musReferenceKeysVisible = new List<string>();
    public List<AudioClip> musReferenceValuesVisible = new List<AudioClip>();

    // RELOAD DAMMNIT

    public AudioClip fewTileEffort;
    public AudioClip theTrueCompletionist;
    public AudioMixer mainMixer;
    [Header("Speedrun Items")]
    public TMP_Text speedrunTimer;
    public bool lvlChangedAlready = false;
    private void Awake()
    {
        Debug.LogWarning("AWAKE START — " + Time.realtimeSinceStartup);
        Debug.LogWarning($"AWAKE ON INSTANCE: {GetInstanceID()}, Persisting object: {Instance == this}, Scene: {gameObject.scene.name}");


        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        source = gameObject.GetComponent<AudioSource>();

        // PUT TS HERE
        int j = 0;
        musReference.Clear();
        foreach (string i in mappingHelperSceneNames)
        {
            Debug.Log("Added: " + j + ", " + mappingHelperAudioClip[j].name);
            musReference.Add(mappingHelperSceneNames[j], mappingHelperAudioClip[j]);
            j++;
        }

        AudioClip song = mainGame;
        foreach (string level in completionistMainGame)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = epilogue;
        foreach (string level in completionistEpilogue)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = gauntlets;
        foreach (string level in completionistGauntlets)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = challengeZone;
        foreach (string level in completionistChallengeZone)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = hardcore;
        foreach (string level in completionistHardcore)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = tower;
        foreach (string level in completionistTower)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = rooms;
        foreach (string level in completionistRooms)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = allInOne;
        foreach (string level in completionistAllInOne)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        if (speedrunTimer != null)
        {
            DontDestroyOnLoad(speedrunTimer.gameObject);
        }

        musReferenceKeysVisible = musReference.Keys.ToList();
        musReferenceValuesVisible = musReference.Values.ToList();


        Debug.LogWarning("AWAKE END — " + Time.realtimeSinceStartup);


    }

    void OnDestroy() {
        Debug.LogError($"DESTROYED INSTANCE {GetInstanceID()} — musReference.Count = {musReference?.Count ?? -1}");
    }

    

    void Start()
    {
        Debug.LogWarning($"START ON INSTANCE: {GetInstanceID()}, musReference.Count = {musReference.Count}");
        LoadDeathCount();
        LoadDashCount();
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "tracker.json")))
        {
            ResetTrackerDotJson("tracker.json", new Dictionary<string, string>(), false);
        }
        else if (string.IsNullOrWhiteSpace(File.ReadAllText(Path.Combine(Application.persistentDataPath, "tracker.json"))))
        {
            ResetTrackerDotJson("tracker.json", new Dictionary<string, string>(), false);
        }


        speedrunTimer.text = "";

        currentScene = SceneManager.GetActiveScene().name;
        if (source.clip != musReference[currentScene])
        {
            source.clip = musReference[currentScene];
            source.loop = true;
            source.Play();
            Debug.Log("Starting music " + source.clip.name + " in " + currentScene);
        }
        else
        {
            source.Play();
        }

        string printer = "";
        foreach (string s in musReference.Keys)
        {
            printer += $"{s}, ";
        }
        Debug.LogWarning(printer);

        keyCodes.AddRange(KeyCode.GetValues(typeof(KeyCode)).Cast<KeyCode>());
    }

    public void ResetTrackerDotJson(string fileName, Dictionary<string, string> recordsToReset, bool justResetSceneStuff)
    {
        if (fileName == "tracker.json" && justResetSceneStuff == false)
        {

            var defaultTracker = new Dictionary<string, object>
            {
                { "deathCount", totalDeathCount },
                { "dashesUsed", totalDashCount },
                { "playerColor", currentPlayerColor },
                { "mainGameLastLevel", mainGameID },
                { "mainGameCompLastLevel", mainGameCompID },
                { "epilogueLastLevel", epilogueID },
                { "epilogueCompLastLevel", epilogueCompID },
                { "hardcoreLastLevel", hardcoreID },
                { "hardcoreCompLastLevel", hardcoreCompID },
                { "challengeZoneLastLevel", challengeZoneID },
                { "challengeZoneCompLastLevel", challengeZoneCompID },
                { "trialsLastLevel", trialsID },
                { "gauntletsLastLevel", gauntletsID },
                { "gauntletsCompLastLevel", gauntletsCompID },
                { "towerLastLevel", towerID },
                { "towerCompLastLevel", towerCompID },
                { "roomsLastLevel", roomsID },
                { "roomsCompLastLevel", roomsCompID },
                { "allInOneLastLevel", allInOneID },
                { "currentSaveFileName", "achievements.json" },
                { "allApplicableSaveFiles", "achievements.json" },
                { "switchTilesKeybind", "Z"},
                { "dashKeybind", "W"},
                { "jumpKeybind", "Space"},
                { "grappleKeybind", "Q"},
                { "reloadKeybind", "R"},
                { "nextLevelKeybind", "N"},
                { "previousLevelKeybind", "B"},
                { "enterPracticeModeKeybind", "P"},
            };
            string finalJSON = Newtonsoft.Json.JsonConvert.SerializeObject(defaultTracker, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "tracker.json"), finalJSON);
            Debug.Log("Woohoo! Reset tracker.json!");
        }
        else if (fileName == "tracker.json" && justResetSceneStuff == true)
        {
            var defaultTracker = new Dictionary<string, object>
            {
                { "currentSaveFileName", "achievements.json" },
                { "allApplicableSaveFiles", "achievements.json" }
            }; 
            string finalJSON = Newtonsoft.Json.JsonConvert.SerializeObject(defaultTracker, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "tracker.json"), finalJSON);
        }
        else
        {
            foreach (KeyValuePair<string, string> kvp in recordsToReset)
            {
                if (kvp.Value == "string")
                {
                    AchievementManager.Instance.WriteToFile(fileName, "", kvp.Key);
                }
                else if (kvp.Value == "int")
                {
                    AchievementManager.Instance.WriteToFile(fileName, 0, kvp.Key);
                }
                else if (kvp.Value == "float")
                {
                    AchievementManager.Instance.WriteToFile(fileName, 0f, kvp.Key);
                }
            }
        }

    }

    public void EnqueuePopup(string message)
    {
        popupQueue.Enqueue(message);
        if (!processingQueue)
        {
            StartCoroutine(ProcessPopupQueue());
        }
    }

    IEnumerator ProcessPopupQueue()
    {
        processingQueue = true;
        while (popupQueue.Count > 0)
        {
            try
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                popupParent = canvas.transform;
            }
            catch (NullReferenceException)
            {

                Debug.LogError("Failed to assign canvas");
            }
            string message = popupQueue.Dequeue();
            Debug.Log(message);
            // Instantiate your popup prefab:
            GameObject popup = Instantiate(popupPrefab, popupParent);
            Debug.Log("Instantiated Popup Prefab");
            Debug.Log(popupParent.childCount + " and is it a child of it? That claim is " + popup.transform.IsChildOf(popupParent).ToString());
            // Set the message on the popup's Text component:
            popup.GetComponentInChildren<TMP_Text>().text = message;
            Debug.Log("Set Message to " + message);
            // Wait for 2 seconds:
            yield return new WaitForSeconds(2f);
            Debug.Log("done Waiting!");
            // Destroy the popup:
            Destroy(popup);
        }
        processingQueue = false;
    }

    public void LoadDeathCount() {
        string path = Path.Combine(Application.persistentDataPath, "tracker.json");
        if (File.Exists(path)) {
            /* 
            DeathData deathsToLoad = JsonUtility.FromJson<DeathData>(File.ReadAllText(path));
            totalDeathCount = deathsToLoad.deathCount;
            Debug.Log("Loaded in DeathData from tracker.json file: " + totalDeathCount);
            */
            totalDeathCount = AchievementManager.Instance.ReadFromFile(path, "deathCount");
            Debug.Log("Loaded in DeathData from tracker.json file: " + totalDeathCount);

        } else {
            Debug.Log("File does not exist, setting value to 0 instead.");
            totalDeathCount = 0;
        }
    }
    public void LoadDashCount() {
        string path = Path.Combine(Application.persistentDataPath, "tracker.json");
        if (File.Exists(path)) {
            totalDashCount = AchievementManager.Instance.ReadFromFile(path, "dashesUsed");
            Debug.Log("Loaded in Dash Count from tracker.json file: " + totalDashCount);
        } else {
            Debug.Log("File does not exist, setting value to 0 instead.");
            totalDashCount = 0;
        }
    }



    IEnumerator waitToAddDeathCount() {
        if (timerOff == true) {

            timerOff = false;

            SaveDeathCount();
            CheckDeathAchievements();
            totalDeathCount++;

            yield return new WaitForSeconds(timeToWait);
            timerOff = true;
        } else {
            Debug.Log("Timer still on!");
        }
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(waitToAddDeathCount());
    }

    private void SaveDeathCount()
    {
        /*
        string fileName = "tracker.json";
        DeathData data = new DeathData();
        data.deathCount = totalDeathCount;
        
        string json = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        File.WriteAllText(filePath, json);
        Debug.Log("Death count saved: " + json + " to path " + filePath);
        */

        DeathData data = new DeathData();
        data.deathCount = totalDeathCount;
        string path = Path.Combine(Application.persistentDataPath, "tracker.json");
        AchievementManager.Instance.WriteToFile(path, data.deathCount, "deathCount");
    }

    private void SaveDashes()
    {
        /*
        string fileName = "tracker.json";
        DeathData data = new DeathData();
        data.deathCount = totalDeathCount;
        
        string json = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        File.WriteAllText(filePath, json);
        Debug.Log("Death count saved: " + json + " to path " + filePath);
        */


        string path = Path.Combine(Application.persistentDataPath, "tracker.json");
        AchievementManager.Instance.WriteToFile(path, totalDashCount, "dashesUsed");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (runActive)
            {
                ForceEndRun();
            }
            if (isPlayingModPack)
            {
                isPlayingModPack = false;
            }
            currentLevelIndex = 0;
            SceneManager.LoadScene("Main Menu");

        }

        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.enterPracticeModeKey)){   
            if (!inPracticeMode)
            {
                inPracticeMode = true;
                EnqueuePopup("Enabled Pratice Mode");
            } else {
                SceneManager.LoadScene("Main Menu");
                inPracticeMode = false;
                EnqueuePopup("Disabled Practice Mode");
                EnqueuePopup("Returned to menu to avoid cheating");
            }
        }   

        

        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.previousLevelKey) && inPracticeMode)
        {
            if (endSceneNames.Contains(SceneManager.GetActiveScene().name))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
        }

       

        if (Input.GetKeyDown(KeyCode.E))
        {
            source.Stop();
            Debug.LogWarning($"Reloading scene {SceneManager.GetActiveScene().name}'s music, which is {source.clip.name}");
            source.Play();
        }

        if (runActive && speedrunTimer != null)
        {
            float t = Time.time - runStartTime;
            speedrunTimer.text = $"{(int)Math.Floor(t / 3600):D2}:{(int)Math.Floor((t % 3600) / 60):D2}:{(int)Math.Floor((t % 3600) % 60):D2}";
        }
    }

    void CheckDeathAchievements()
    {
        if (totalDeathCount >= 1000)
        {
            if (!AchievementManager.Instance.IsUnlocked("AT_LEAST_YOU_TRIED")) {
                EnqueuePopup("Unlocked Achievement 'At Least You Tried'");
            }
            AchievementManager.Instance.UnlockAchievement("AT_LEAST_YOU_TRIED");

        }
        if (totalDeathCount >= 10000)
        {
            if (!AchievementManager.Instance.IsUnlocked("OK_YOU_JUST_SUCK")) {
                EnqueuePopup("Unlocked Achievement 'Ok, You Just Suck'");
            }
            AchievementManager.Instance.UnlockAchievement("OK_YOU_JUST_SUCK");
        }
        if (totalDeathCount >= 50000)
        {
            if (!AchievementManager.Instance.IsUnlocked("YOU_TRIED_TO_GET_THIS")) {
                EnqueuePopup("Unlocked Achievement 'You TRIED to Get This'");
            }
            AchievementManager.Instance.UnlockAchievement("YOU_TRIED_TO_GET_THIS"); ;
        }
    }

    public void UnlockSecret() {
        AchievementManager.Instance.UnlockAchievement("MASTER_DETECTIVE");
        EnqueuePopup("Unlocked Achievement 'Master Detective'");
    }


    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    // ----------------------------------------------------
    // 1) Start a run in a certain mode
    // ----------------------------------------------------
    public void StartRun(GameMode mode)
    {
        if (runActive)
        {
            Debug.LogWarning("A run is already active; forcibly ending it first.");
            EndRun(false);
        }

        runActive = true;
        currentMode = mode;
        runStartTime = Time.time;

        dashUsedThisRun = false;
        grappleCountThisRun = 0;
        reachedHighBounce = false;

        Debug.Log("Run started! Mode: " + mode);
    }

    // ----------------------------------------------------
    // 2) Scene-based detection for finishing a mode properly
    // ----------------------------------------------------
    private void OnActiveSceneChanged(Scene oldScene, Scene scene)
    {
        string loadedName = scene.name;
        int buildIndex = scene.buildIndex;
        Debug.LogWarning($"LOADING INTO SCENE {loadedName}, FROM SCENE {oldScene.name}");

        musReferenceKeysVisible = musReference.Keys.ToList();
        musReferenceValuesVisible = musReference.Values.ToList();

        if (loadedName != "Main Menu")
        {
            Console.WriteLine("Latest Level Saved As " + loadedName);
            PlayerPrefs.SetString("LatestLevel", loadedName);
        }

        if (loadedName == "Main Menu")
        {
            AchievementManager.Instance.LoadAchievementsFromFile();
            deathText = GameObject.FindGameObjectWithTag("deathText").GetComponent<TMP_Text>();
            saveFileText = GameObject.FindGameObjectWithTag("saveFileText").GetComponent<TMP_Text>();
            int myDeathCount = AchievementManager.Instance.ReadFromFile("tracker.json", "deathCount");
            if (myDeathCount > 0) deathText.text = "You've died: " + myDeathCount + " times!\n";
            if (myDeathCount == 0) deathText.text = "You've never died!\n";
            if (totalDeathCount < 0) deathText.text = "Kris Get The Banana\n";
            saveFileText.text = $"Savefile Name: {string.Join("", AchievementManager.Instance.achievementsFileName.Replace(".json", "").ToCharArray().ToList().Select(p => AchievementManager.Instance.achievementsFileName.Replace(".json", "").ToCharArray().ToList().IndexOf(p) == 0 ? char.ToUpper(p) : p).ToArray())}";

            int count = 0;
            int compareCount = AchievementManager.Instance.GetAllAchievements().Count;
            Debug.LogWarning($"Achievement Count = {AchievementManager.Instance.GetAllAchievements().Count}");
            foreach (AchievementInfo achievement in AchievementManager.Instance.GetAllAchievements())
            {
                if (AchievementManager.Instance.IsUnlocked(achievement.achievementID))
                {
                    count++;
                }
            }
            percentText = GameObject.FindGameObjectWithTag("percentText").GetComponent<TMP_Text>();
            Debug.LogWarning($"Achievement Count: {count}");
            Debug.LogWarning($"Achievement Compare Count: {compareCount}");
            percentText.text = $"You are {Mathf.RoundToInt(100 * ((float)count/(float)compareCount))}% through the game!";
            if (count == compareCount && compareCount == 29)
            {
                musReference["Main Menu"] = theTrueCompletionist;
                musReference["StartOver"] = theTrueCompletionist;
                mappingHelperAudioClip[96] = theTrueCompletionist;
                mappingHelperAudioClip[97] = theTrueCompletionist;
                Debug.LogWarning("All Achievements were unlocked!");
            }
            else
            {
                musReference["Main Menu"] = fewTileEffort;
                musReference["StartOver"] = fewTileEffort;
                mappingHelperAudioClip[96] = fewTileEffort;
                mappingHelperAudioClip[97] = fewTileEffort;
                Debug.LogWarning($"Not all achievements were unlocked. Only unlocked {count} achievements, while there are {compareCount} in total. Missing {compareCount - count}.");
            }
            Debug.LogWarning($"The Main Menu song is currently {musReference["Main Menu"].name} (Triggered New Track: {count == compareCount})");

        }
        
        if (!runActive || currentMode == GameMode.None || currentMode == GameMode.NotAMode)
        {
            Debug.LogWarning("No mode active!");
        }
        else if(!inPracticeMode)
        { 
            switch (currentMode)
            {
                case GameMode.MainGame:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "mainGameLastLevel");
                    if (loadedName == mainGameEndScene) EndRun(true);
                    break;
                case GameMode.MainGameComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "mainGameCompLastLevel");
                    if (loadedName == mainGameCompEndScene) EndRun(true);
                    break;
                case GameMode.Epilogue:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "epilogueLastLevel");
                    if (loadedName == epilogueEndScene) EndRun(true);
                    break;
                case GameMode.EpilogueComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "epilogueCompLastLevel");
                    if (loadedName == epilogueCompEndScene) EndRun(true);
                    break;
                case GameMode.Hardcore:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "hardcoreLastLevel");
                    if (loadedName == hardcoreEndScene) EndRun(true);
                    break;
                case GameMode.HardcoreComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "hardcoreCompLastLevel");
                    if (loadedName == hardcoreCompEndScene) EndRun(true);
                    break;
                case GameMode.ChallengeZone:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "challengeZoneLastLevel");
                    if (loadedName == challengeZoneEndScene) EndRun(true);
                    break;
                case GameMode.ChallengeZoneComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "challengeZoneCompLastLevel");
                    if (loadedName == challengeZoneCompEndScene) EndRun(true);
                    break;
                case GameMode.Trials:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "trialsLastLevel");
                    if (loadedName == trialsEndScene) EndRun(true);
                    break;
                case GameMode.Gauntlets:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "gauntletsLastLevel");
                    if (loadedName == gauntletsEndScene) EndRun(true);
                    break;
                case GameMode.GauntletsComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "gauntletsCompLastLevel");
                    if (loadedName == gauntletsCompEndScene) EndRun(true);
                    break;
                case GameMode.Tower:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "towerLastLevel");
                    if (loadedName == towerEndScene) EndRun(true);
                    break;
                case GameMode.AllInOne:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "allInOneLastLevel");
                    if (loadedName == allInOneEndScene) EndRun(true);
                    break;
                case GameMode.TowerComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "towerCompLastLevel");
                    if (loadedName == towerCompEndScene) EndRun(true);
                    break;
                case GameMode.Rooms:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "roomsLastLevel");
                    if (loadedName == roomsEndScene) EndRun(true);
                    break;
                case GameMode.RoomsComp:
                    AchievementManager.Instance.WriteToFile("tracker.json", buildIndex, "roomsCompLastLevel");
                    if (loadedName == roomsCompEndScene) EndRun(true);
                    break;
            }
        }
        if (source == null) source = gameObject.GetComponent<AudioSource>();
        if (source.clip == null) source.clip = musReference["Main Menu"];

        source = gameObject.GetComponent<AudioSource>();
        int j = 0;
        musReference.Clear();
        foreach (string i in mappingHelperSceneNames)
        {
            Debug.Log("Added: " + j + ", " + mappingHelperAudioClip[j].name);
            musReference.Add(mappingHelperSceneNames[j], mappingHelperAudioClip[j]);
            j++;
        }

           AudioClip song = mainGame;
        foreach (string level in completionistMainGame)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = epilogue;
        foreach (string level in completionistEpilogue)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = gauntlets;
        foreach (string level in completionistGauntlets)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = challengeZone;
        foreach (string level in completionistChallengeZone)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = hardcore;
        foreach (string level in completionistHardcore)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = tower;
        foreach (string level in completionistTower)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = rooms;
        foreach (string level in completionistRooms)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }

        song = allInOne;
        foreach (string level in completionistAllInOne)
        {
            Debug.LogWarning($"Adding {level} to musReference with song {song.name}");
            musReference.Add(level, song);
        }
        Debug.LogWarning("AFTER ASSIGNMENT, musReference.Count = " + musReference.Count);

        if (source.clip != musReference[scene.name])
        {
            Debug.LogWarning($"ENTERED FIRST BLOCK IN SCENE {scene.name}");
            Debug.LogWarning($"MUSREFERENCE INCLUDES SCENE NAME: {musReference.ContainsKey(scene.name)}");
            if (musReference.ContainsKey(scene.name))
            {
                source.clip = musReference[scene.name];
                source.loop = true;
                source.Play();
                Debug.LogWarning("Starting music " + source.clip.name + " in " + scene.name);

            }
            else
            {
                // update scripts
                source.Stop();
                Debug.Log("Stopped music as key: " + scene.name + " was not contained in the music map. If this was intentional, don't worry!");
                Debug.Log("Key Value: " + musReference.ContainsKey(scene.name));
            }
        }
        else if (scene.name == "Main Menu" || scene.name == "StartOver")
        {
            Debug.LogWarning($"TRIGGERED SECONDARY LOOP!! currentScene={oldScene.name}, scene.name={scene.name}");
            if (!source.isPlaying || source.clip != musReference[scene.name])
            {
                source.clip = musReference[scene.name];
                source.loop = true;
                source.Play();
                // force asset refresh
                Debug.LogWarning("Starting music " + source.clip.name + " in " + scene.name);
            }
        }
        else if (source.clip == musReference[scene.name])
        {
            Debug.LogWarning("Reloading Scene- music should not stop!");
        }
        else
        {
            Debug.LogWarning("Something has gone horribly wrong");
        }

        Debug.LogWarning($"Music logic complete! isPlaying={source.isPlaying}, Current Loaded Clip={source.clip.name}");

        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "jumpKeybind", true), out KeyCode QparsedKey);
        jumpKey = QparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "dashKeybind", true), out KeyCode WparsedKey);
        dashKey = WparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "grappleKeybind", true), out KeyCode EparsedKey);
        grappleKey = EparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "reloadKeybind", true), out KeyCode RparsedKey);
        reloadKey = RparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "nextLevelKeybind", true), out KeyCode TparsedKey);
        nextLevelKey = TparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "previousLevelKeybind", true), out KeyCode YparsedKey);
        previousLevelKey = YparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "switchTilesKeybind", true), out KeyCode UparsedKey);
        switchTilesKey = UparsedKey;
        System.Enum.TryParse(AchievementManager.Instance.ReadFromFile("tracker.json", "enterPracticeModeKeybind", true), out KeyCode HparsedKey);
        enterPracticeModeKey = HparsedKey;
        lvlChangedAlready = false;

    } 

    // ----------------------------------------------------
    // 3) Force-end the run => not completed properly
    // ----------------------------------------------------
    public void ForceEndRun()
    {
        EndRun(false);
    }

    // ----------------------------------------------------
    // 4) EndRun => TOTALLY_DASHLESS (only if mode is MainGame/Epilogue/Hardcore, dashless, and completedProperly)
    //    plus Grapple Master, GalaxyFarAway, finishing/time-based achievements
    // ----------------------------------------------------
    private void EndRun(bool completedProperly)
    {
        if (!runActive) return;
        
        runActive = false;

        source.Stop();
        isPlayingMusic = false;
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            // GameObject.Destroy(speedrunTimer.gameObject.transform.parent.gameObject);
            // speedrunTimer = GameObject.Find("Speedrun Timer").GetComponent<TMP_Text>();
            speedrunTimer.text = "";
            if (musReference.ContainsKey("Main Menu"))
            {
                source.clip = musReference["Main Menu"];
                source.Play();
                isPlayingMusic = true;
            }
        } 

        float totalTime = Time.time - runStartTime;
        speedrunTimer.text = "";
        Debug.Log($"Run ended! Mode={currentMode}, time={totalTime:F2}, dashUsed={dashUsedThisRun}, grapples={grappleCountThisRun}, bounce={reachedHighBounce}, completedProperly={completedProperly}");

        if(!inPracticeMode){
            // Grapple Master => awarded whenever the run ends if 10+ grapples
            if (grappleCountThisRun >= 10)
            {
                if(!AchievementManager.Instance.IsUnlocked("GRAPPLE_MASTER")){
                    EnqueuePopup("Unlocked Achievement 'Grapple Master'");
                }
                AchievementManager.Instance.UnlockAchievement("GRAPPLE_MASTER");

            }

            // Galaxy Far Away
            if (reachedHighBounce)
            {
                if(!AchievementManager.Instance.IsUnlocked("GALAXY_FAR_AWAY")){
                    EnqueuePopup("Unlocked Achievement 'Galaxy Far Away'");
                }
                AchievementManager.Instance.UnlockAchievement("GALAXY_FAR_AWAY");
            }

            //Unlock Super Dasher if the total dash count hits 500
            if (totalDashCount >= 500){
                if(!AchievementManager.Instance.IsUnlocked("SUPER_DASHER")){
                    EnqueuePopup("Unlocked Achievement 'Super Dasher'");
                }
                AchievementManager.Instance.UnlockAchievement("SUPER_DASHER");
            }

            // If the run didn't properly complete the mode => skip finishing/time-based achievements
            if (!completedProperly)
            {
                Debug.Log("Run ended forcibly => no finishing/time achievements, TOTALLY_DASHLESS partial checks not applied.");
                currentMode = GameMode.None;
                return;
            }

            // If we DID finish properly, let's do the big achievements:

            // Totally Dashless partial logic => only if dashUsed==false AND mode is mainGame/epilogue/hardcore normal
            //   but we only set the partial flags if completedProperly
            if (!dashUsedThisRun)
            {
                if (currentMode == GameMode.MainGame) mainGameDashlessDone = true;
                if (currentMode == GameMode.Epilogue) epilogueDashlessDone = true;
                if (currentMode == GameMode.Hardcore) hardcoreDashlessDone = true;
                if (currentMode == GameMode.MainGameComp) mainGameCompDashlessDone = true;
                if (currentMode == GameMode.EpilogueComp) epilogueCompDashlessDone = true;
                if (currentMode == GameMode.HardcoreComp) hardcoreCompDashlessDone = true;

                // If all 3 are done => Totally Dashless
                if (mainGameDashlessDone && epilogueDashlessDone && hardcoreDashlessDone)
                {
                    AchievementManager.Instance.UnlockAchievement("TOTALLY_DASHLESS");
                    EnqueuePopup("Unlocked Achievement 'Totally Dashless'");
                }

                // If all 3 are done => The Dashless Express
                if (mainGameCompDashlessDone && epilogueCompDashlessDone && hardcoreCompDashlessDone)
                {
                    AchievementManager.Instance.UnlockAchievement("THE_DASHLESS_EXPRESS");
                    EnqueuePopup("Unlocked Achievement 'The Dashless Express'");
                }
            }
        
            // Time-based & finishing achievements
            switch (currentMode)
            {
                case GameMode.MainGame:
                    if (totalTime <= 600f){ // Speed Demon
                        if(!AchievementManager.Instance.IsUnlocked("SPEED_DEMON")){
                            EnqueuePopup("Unlocked Achievement 'Speed Demon'");
                        }
                        AchievementManager.Instance.UnlockAchievement("SPEED_DEMON");
                    }
                    if (totalTime <= 300f){ // Double Time
                        if(!AchievementManager.Instance.IsUnlocked("DOUBLE_TIME")){
                            EnqueuePopup("Unlocked Achievement 'Double Time'");
                        }
                        AchievementManager.Instance.UnlockAchievement("DOUBLE_TIME");
                        
                    }

                    // We All Start Somewhere
                    if(!AchievementManager.Instance.IsUnlocked("WE_ALL_START_SOMEWHERE"))
                    {
                        EnqueuePopup("Unlocked Achievement 'We All Start Somewhere'");
                        EnqueuePopup("Unlocked Epilogue");
                        EnqueuePopup("Unlocked Completionist Main Game");
                    }
                    AchievementManager.Instance.UnlockAchievement("WE_ALL_START_SOMEWHERE");
                    break;

                case GameMode.Epilogue:
                    // Bittersweet
                    if(!AchievementManager.Instance.IsUnlocked("BITTERSWEET"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Bittersweet'");
                        EnqueuePopup("Unlocked Completionist Epilogue");
                    }
                    AchievementManager.Instance.UnlockAchievement("BITTERSWEET");
                    break;

                case GameMode.Hardcore:
                    if (totalTime <= 600f)
                    {
                        if(!AchievementManager.Instance.IsUnlocked("GOTTA_GO_FAST"))
                        {
                            EnqueuePopup("Unlocked Achievement 'Gotta Go Fast'");
                        }
                        AchievementManager.Instance.UnlockAchievement("GOTTA_GO_FAST");
                        
                    }
                    // Timings Galore
                    if(!AchievementManager.Instance.IsUnlocked("TIMINGS_GALORE"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Timings Galore'");
                        EnqueuePopup("Unlocked The Tower");
                        EnqueuePopup("Unlocked Completionist Hardcore");
                    }
                    AchievementManager.Instance.UnlockAchievement("TIMINGS_GALORE");
                    break;

                case GameMode.ChallengeZone:
                    // Getting Somewhere
                    if(!AchievementManager.Instance.IsUnlocked("GETTING_SOMEWHERE"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Getting Somewhere'");
                        EnqueuePopup("Unlocked Hardcore");
                        EnqueuePopup("Unlocked Completionist Challenge Zone");
                    }
                    AchievementManager.Instance.UnlockAchievement("GETTING_SOMEWHERE");
                    break;

                case GameMode.Trials:
                    // Nowhere but Up
                    if(!AchievementManager.Instance.IsUnlocked("NOWHERE_BUT_UP"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Nowhere but Up'");
                        EnqueuePopup("Unlocked The Gauntlets");
                    }
                    AchievementManager.Instance.UnlockAchievement("NOWHERE_BUT_UP");
                    break;

                case GameMode.Gauntlets:
                    // New Features?
                    if(!AchievementManager.Instance.IsUnlocked("NEW_FEATURES"))
                    {
                        EnqueuePopup("Unlocked Achievement 'New Features?'");
                        EnqueuePopup("Unlocked Challenge Zone");
                        EnqueuePopup("Unlocked Completionist Challenge Zone");
                    }
                    AchievementManager.Instance.UnlockAchievement("NEW_FEATURES");
                    break;

                case GameMode.Tower:
                    // Tower Conqueror
                    if(!AchievementManager.Instance.IsUnlocked("TOWER_CONQUEROR"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Tower Conqueror'");
                        EnqueuePopup("Unlocked The Rooms");
                        EnqueuePopup("Unlocked Completionist All In One");
                        EnqueuePopup("Unlocked Completionist Tower");
                    }
                    AchievementManager.Instance.UnlockAchievement("TOWER_CONQUEROR");
                    break;

                case GameMode.AllInOne:
                    // The True Ending
                    if(!AchievementManager.Instance.IsUnlocked("THE_TRUE_ENDING"))
                    {
                        EnqueuePopup("Unlocked Achievement 'The True Ending'");
                    }
                    AchievementManager.Instance.UnlockAchievement("THE_TRUE_ENDING");
                    break;

                case GameMode.MainGameComp:
                    // Start of Hard
                    if(!AchievementManager.Instance.IsUnlocked("START_OF_HARD"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Start of Hard'");
                    }
                    AchievementManager.Instance.UnlockAchievement("START_OF_HARD");
                    break;

                case GameMode.EpilogueComp:
                    // Not Again...
                    if(!AchievementManager.Instance.IsUnlocked("NOT_AGAIN"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Not Again'");
                    }
                    AchievementManager.Instance.UnlockAchievement("NOT_AGAIN");
                    break;

                case GameMode.ChallengeZoneComp:
                    // Too Hard 4 U
                    if(!AchievementManager.Instance.IsUnlocked("TOO_HARD_4_U"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Too Hard 4 U'");
                    }
                    AchievementManager.Instance.UnlockAchievement("TOO_HARD_4_U");
                    break;

                case GameMode.HardcoreComp:
                    // Going Into Overdrive <=600
                    if (totalTime <= 600f){
                        if(!AchievementManager.Instance.IsUnlocked("GOING_INTO_OVERDRIVE"))
                        {
                            EnqueuePopup("Unlocked Achievement 'Going Into Overdrive'");
                        }
                        AchievementManager.Instance.UnlockAchievement("GOING_INTO_OVERDRIVE");
                    }
                    // Expert++
                    if(!AchievementManager.Instance.IsUnlocked("EXPERT_PLUS_PLUS"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Expert++'");
                    }
                    AchievementManager.Instance.UnlockAchievement("EXPERT_PLUS_PLUS");
                    break;
                case GameMode.TowerComp:
                    // Give the player Ascention to New Heights
                    if(!AchievementManager.Instance.IsUnlocked("ASCENTION_TO_NEW_HEIGHTS"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Ascention to New Heights'");
                    }
                    AchievementManager.Instance.UnlockAchievement("ASCENTION_TO_NEW_HEIGHTS");
                    break;
                case GameMode.Rooms:
                    if(!AchievementManager.Instance.IsUnlocked("KING_OF_PAIN"))
                    {
                        EnqueuePopup("Unlocked Achievement 'King of Pain'");
                        EnqueuePopup("Unlocked Completionist Rooms");
                    }
                    AchievementManager.Instance.UnlockAchievement("KING_OF_PAIN");
                    break;
                case GameMode.RoomsComp:
                    if(!AchievementManager.Instance.IsUnlocked("ALMOST_THERE"))
                    {
                        EnqueuePopup("Unlocked Achievement 'Almost There'");
                    }
                    AchievementManager.Instance.UnlockAchievement("ALMOST_THERE");
                    break;
                case GameMode.GauntletsComp:
                    if(!AchievementManager.Instance.IsUnlocked("NEW_FEATURES_AGAIN"))
                    {
                        EnqueuePopup("Unlocked Achievement 'New Features.... Again?'");
                    }
                    AchievementManager.Instance.UnlockAchievement("NEW_FEATURES_AGAIN");
                    break;
            }
        }

        currentMode = GameMode.None;
    }

    // ----------------------------------------------------
    // Called from dash script if a dash is used
    // ----------------------------------------------------
    public void OnDashUsed()
    {
        if (runActive) dashUsedThisRun = true;
        totalDashCount++;
        SaveDashes();
    }

    // ----------------------------------------------------
    // Called from grapple script on success
    // ----------------------------------------------------
    public void OnGrappleSuccess()
    {
        if (runActive) grappleCountThisRun++;
    }

    // ----------------------------------------------------
    // Called from bounce logic if y > galaxyBounceThreshold
    // ----------------------------------------------------
    public void CheckBounceHeight(float currentY)
    {
        if (runActive && !reachedHighBounce && currentY > galaxyBounceThreshold)
        {
            reachedHighBounce = true;
        }
    }
}
