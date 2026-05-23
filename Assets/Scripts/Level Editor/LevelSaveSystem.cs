using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // For reloading the scene
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using SFB; // Standalone File Browser
using TMPro;
using System.Linq;

public class LevelSaveManager : MonoBehaviour
{
    [Header("Scene Objects")]
    public Transform objectParent; // Parent that holds placed objects

    [Header("Key Shortcuts")]
    public KeyCode autoSaveKey = KeyCode.X;   // Ctrl+X
    public KeyCode manualSaveKey = KeyCode.S; // Ctrl+S
    public KeyCode saveAsKey = KeyCode.A;     // Ctrl+A
    public KeyCode loadKey = KeyCode.L;       // Ctrl+L
    public KeyCode loadBlankLevelKey = KeyCode.R;  // Ctrl+R

    private List<GameObject> placedObjects = new List<GameObject>();
    private List<GameObject> prefabList; // from LevelEditorManager

    // We'll store the full path to the current file here:
    private string currentFilePath = ""; 

    private int fileNumber = 1; // For auto-save

    // PlayerPrefs keys
    private const string FILE_PATH_KEY = "LastFilePath";
    private const string LAST_FILE_NUMBER_KEY = "LastFileNumber";
    public bool isTutorial = false;

    ////////////////////////////////////////////////////////////////////////////////
    //                          UNITY LIFECYCLE
    ////////////////////////////////////////////////////////////////////////////////

    void Awake()
    {
        // Ensure an EventSystem in case your UI needs it
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Retrieve prefabList from LevelEditorManager
        LevelEditorManager levelEditorManager = GetComponent<LevelEditorManager>();
        if (levelEditorManager != null)
        {
            prefabList = levelEditorManager.prefabList;
        }
        else
        {
            Debug.LogError("LevelEditorManager not found on this GameObject!");
        }

        // Restore last saved file path & fileNumber from PlayerPrefs
        LoadLastFilePath();
        LoadLastFileNumber();

        // If we have a valid file path (not "BLANK"), auto-load the level
        if (!string.IsNullOrEmpty(currentFilePath) && currentFilePath != "BLANK")
        {
            if (File.Exists(currentFilePath))
            {
                Debug.Log("Reloading last level from: " + currentFilePath);
                LoadLevelFromPath_Internal(currentFilePath);
            }
            else
            {
                // The file no longer exists - let's just blank out
                Debug.LogWarning("File not found, loading blank instead.");
                LoadBlankLevel();
            }
        }
        else
        {
            Debug.Log("No saved file path or it's BLANK, starting empty scene.");
            // Scene remains empty
        }
    }

    void Update()
    {
        // CTRL + X => Auto-save to persistentDataPath with an incremented number
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(autoSaveKey))
        {
            SaveLevelWithNumber();
        }

        // CTRL + S => "Save" (overwrite if we have a path, else prompt "Save As")
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(manualSaveKey))
        {
            SaveLevel();
        }

        // CTRL + A => Always "Save As"
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(saveAsKey))
        {
            SaveLevelAs();
        }

        // CTRL + L => Load from file
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(loadKey))
        {
            OpenFileBrowserToLoadLevel();
        }

        // CTRL + R => Load blank level
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(loadBlankLevelKey)){
            LoadBlankLevel();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    //                          DEATH & SCENE RELOAD
    ////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Call this when the player dies. It reloads the current scene,
    /// which triggers Awake() to auto-load the last level if any.
    /// </summary>
    public void OnPlayerDeath()
    {
        Debug.LogWarning("Player died. Reloading scene...");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    ////////////////////////////////////////////////////////////////////////////////
    //                          SAVE / SAVE AS
    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// If we already have a file path (and it's not "BLANK"), overwrite.
    /// Otherwise, prompt "Save As."
    /// </summary>
    public void SaveLevel()
    {
        if (string.IsNullOrEmpty(currentFilePath) || currentFilePath == "BLANK")
        {
            SaveLevelAs();
            return;
        }

        SaveLevelToPath(currentFilePath);
    }

    /// <summary>
    /// Always opens a file browser to pick a new location/filename, 
    /// then updates currentFilePath.
    /// </summary>
    public void SaveLevelAs()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Platfomer Fun Levels", "lvl"),
            new ExtensionFilter("Text Files", "txt"),
            new ExtensionFilter("All Files", "*" )
        };

        string defaultName = "Level" + fileNumber;

        // Show "Save" dialog
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save Level As...", "", defaultName, extensions);

        if (!string.IsNullOrEmpty(filePath))
        {
            UpdateCurrentFilePath(filePath);
            SaveLevelToPath(filePath);
            fileNumber++;
            SaveLastFileNumber();
        }
        else
        {
            Debug.Log("Save As canceled or invalid path.");
        }
    }

    /// <summary>
    /// Original auto-save approach to persistentDataPath with incremented number
    /// </summary>
    void SaveLevelWithNumber()
    {
        string numberedFilename = fileNumber.ToString() + ".lvl";
        string filePath = Path.Combine(Application.persistentDataPath, numberedFilename);

        SaveLevelToPath(filePath);

        fileNumber++;
        SaveLastFileNumber();
    }

    void SaveLevelToPath(string path)
    {
        string levelData = GetLevelData();
        try
        {
            File.WriteAllText(path, levelData);
            Debug.Log("Saved to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save: " + e.Message);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    //                          LOAD
    ////////////////////////////////////////////////////////////////////////////////

    // Open native file explorer to load a level
    void OpenFileBrowserToLoadLevel()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Platformer Fun Levels", "lvl"),
            new ExtensionFilter("Text Files", "txt"),
            new ExtensionFilter("All Files", "*" )
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Level to Load", "", extensions, false);

        if (paths.Length > 0)
        {
            string selectedPath = paths[0];
            if (File.Exists(selectedPath))
            {
                LoadLevelFromPath(selectedPath);
            }
            else
            {
                Debug.LogError("Selected file does not exist: " + selectedPath);
            }
        }
    }

    /// <summary>
    /// Public method to load a level from a specified file path.
    /// Also updates currentFilePath so we can reload on scene restart.
    /// </summary>
    public void LoadLevelFromPath(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError("File not found: " + fullPath);
            return;
        }

        UpdateCurrentFilePath(fullPath);
        LoadLevelFromPath_Internal(fullPath);
    }

    /// <summary>
    /// Internal load that doesn't update currentFilePath. Used in Awake for auto-load.
    /// </summary>
    private void LoadLevelFromPath_Internal(string fullPath)
    {
        string jsonData = File.ReadAllText(fullPath);
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData);
        RestoreLevel(levelData);

        Debug.Log("Loaded level from: " + fullPath);
    }

    /// <summary>
    /// Load a blank level (destroys all objects) and sets currentFilePath = "BLANK".
    /// Next time the scene reloads, we won't load anything.
    /// </summary>
    public void LoadBlankLevel()
    {
        // Destroy all existing objects
        foreach (Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }

        UpdateCurrentFilePath("BLANK");
        Debug.Log("Loaded blank level. currentFilePath set to BLANK.");
    }

    ////////////////////////////////////////////////////////////////////////////////
    //                    GATHER & RESTORE LEVEL DATA
    ////////////////////////////////////////////////////////////////////////////////

    string GetLevelData()
    {
        placedObjects.Clear();
        foreach (Transform child in objectParent)
        {
            placedObjects.Add(child.gameObject);
        }

        LevelData levelData = new LevelData();

        foreach (GameObject obj in placedObjects)
        {
            ObjectData objectData = new ObjectData
            {
                position   = obj.transform.position,
                scale      = obj.transform.localScale,
                rotation   = obj.transform.rotation.eulerAngles,
                prefabName = obj.name.Replace("(Clone)", "")
            };

            // MovingTile
            if (obj.TryGetComponent<MovingTile>(out MovingTile mt))
            {
                objectData.moveSpeed = mt.moveSpeed;
                objectData.waypointA = mt.waypoints[0].transform.position;
                objectData.waypointB = mt.waypoints[1].transform.position;
            }

            // FiringTile
            if (obj.TryGetComponent<FiringTile>(out FiringTile ft))
            {
                objectData.fireRate  = ft.fireRate;
                objectData.fireSpeed = ft.fireSpeed;
                objectData.fireAngle = ft.fireAngle;
            }

            // SpawnerTile
            if (obj.TryGetComponent<SpawnerTile>(out SpawnerTile st))
            {
                objectData.isNewPlayer = st.isNewPlayer;
                objectData.prefabToSpawnName =
                    st.prefabToSpawn != null ? st.prefabToSpawn.name : "";
                objectData.spawnPosition = st.spawnAt.transform.position;
            }

            levelData.objects.Add(objectData);
        }

        return JsonUtility.ToJson(levelData, true);
    }

    void RestoreLevel(LevelData levelData)
    {
        // Clear existing
        foreach (Transform child in objectParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate from saved data
        foreach (ObjectData objData in levelData.objects)
        {
            GameObject prefab = prefabList.Find(p => p.name == objData.prefabName);

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, objData.position, Quaternion.Euler(objData.rotation), objectParent);
                obj.transform.localScale = objData.scale;

                if (obj.TryGetComponent<MovingTile>(out MovingTile mt))
                {
                    mt.waypoints[0] = Instantiate(gameObject.GetComponent<LevelEditorManager>().waypointPrefab, objData.waypointA, Quaternion.identity);
                    mt.waypoints[1] = Instantiate(gameObject.GetComponent<LevelEditorManager>().waypointPrefab, objData.waypointB, Quaternion.identity);
                    mt.moveSpeed = objData.moveSpeed;
                }

                if(obj.TryGetComponent<FiringTile>(out FiringTile bst))
                {
                    bst.fireAngle = objData.fireAngle;
                    bst.fireRate = objData.fireRate;
                    bst.RestartFiring();
                    bst.fireSpeed = objData.fireSpeed;
                }

                if(obj.TryGetComponent<SpawnerTile>(out SpawnerTile st))
                {
                    st.prefabToSpawn = prefabList.Find(p => p.name == objData.prefabToSpawnName);
                    st.isNewPlayer = objData.isNewPlayer;
                    st.spawnAt = Instantiate(gameObject.GetComponent<LevelEditorManager>().spawnerTilePrefab, objData.spawnPosition, Quaternion.identity);
                }
            }
            else
            {
                Debug.LogError("Prefab not found in prefab list: " + objData.prefabName);
            }
        }
    }
    

    ////////////////////////////////////////////////////////////////////////////////
    //                          FILE PATH PERSISTENCE
    ////////////////////////////////////////////////////////////////////////////////

    private void UpdateCurrentFilePath(string newPath)
    {
        currentFilePath = newPath;
        PlayerPrefs.SetString(FILE_PATH_KEY, newPath);
        PlayerPrefs.Save();
    }

    private void LoadLastFilePath()
    {
        currentFilePath = PlayerPrefs.GetString(FILE_PATH_KEY, "");
    }

    private void SaveLastFileNumber()
    {
        PlayerPrefs.SetInt(LAST_FILE_NUMBER_KEY, fileNumber);
        PlayerPrefs.Save();
    }

    private void LoadLastFileNumber()
    {
        fileNumber = PlayerPrefs.GetInt(LAST_FILE_NUMBER_KEY, 1);
    }
}

// Serializable classes for saving/loading data
[System.Serializable]
public class ObjectData
{
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;
    public string prefabName;

    // --- MovingTile ---
    public float moveSpeed;
    public Vector3 waypointA;
    public Vector3 waypointB;

    // --- FiringTile ---
    public float fireRate;
    public float fireSpeed;
    public float fireAngle;

    // --- SpawnerTile ---
    public bool isNewPlayer;
    public string prefabToSpawnName;
    public Vector3 spawnPosition;
}

[System.Serializable]
public class LevelData
{
    public List<ObjectData> objects = new List<ObjectData>();
}
