using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.TerrainTools;

public class LevelEditorManager : MonoBehaviour
{
    // List of 2D prefabs available for placement
    public List<GameObject> prefabList;

    // UI elements for selecting prefabs
    public Dropdown prefabDropdown;

    // Reference to the currently selected prefab
    private GameObject selectedPrefab;

    // Parent object for placed objects
    public Transform objectParent;

    // Grid snapping variables
    private bool isGridSnapping = false;
    public float gridSize = 1f; // Size of the grid cells
    public ObjectSelectionManager osm;
    public float sensitivity = 0.8f;
    public float frSensitivity = 0.2f;
    public CinemachineVirtualCamera cam;
    public bool isEditFirstOne = true;
    public GameObject waypointPrefab;
    // This is the prefab for the Spawner Tile Spawn Pos, not the Spawner Tile itself.
    public GameObject spawnerTilePrefab;
    public GameObject playerPrefab;

    void Start()
    {
        osm = GetComponent<ObjectSelectionManager>();
        // Populate dropdown with prefab names
        PopulatePrefabDropdown();
    }

    void Update()
    {
        // Check if the player clicks to place the selected prefab
        if (Input.GetMouseButtonDown(1) && selectedPrefab != null && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if(osm.selectedObject.TryGetComponent<MovingTile>(out MovingTile mt))
                {
                    Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mp.z = 0;
                    GameObject waypoint = Instantiate(waypointPrefab, mp, Quaternion.identity);
                    if (isEditFirstOne){
                        GameObject urDead = mt.waypoints[0];
                        mt.waypoints[0] = waypoint;
                        isEditFirstOne = false;
                        Destroy(urDead);
                    } else {
                      GameObject urDead = mt.waypoints[1];
                      mt.waypoints[1] = waypoint;
                      isEditFirstOne = true;  
                      Destroy(urDead);
                    }
                } else if (osm.selectedObject.TryGetComponent<SpawnerTile>(out SpawnerTile spawner))
                {
                    Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mp.z = 0;
                    GameObject spawnPos = Instantiate(spawnerTilePrefab, mp, Quaternion.identity);
                    spawner.spawnAt = spawnPos;
                }
            } else { 
                PlaceObject();
            }
        }
        float scrollY = Input.mouseScrollDelta.y;
        if(scrollY != 0)
        {
            if(osm.selectedObject.TryGetComponent<MovingTile>(out MovingTile mov))
            {
                mov.moveSpeed += scrollY;
            }

            if(osm.selectedObject.TryGetComponent<FiringTile>(out FiringTile bst))
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    bst.fireRate = (bst.fireRate + scrollY * sensitivity) < 0.1 ? 0.1f : bst.fireRate + scrollY * sensitivity;
                    bst.RestartFiring();
                } else if (Input.GetKey(KeyCode.Alpha2))
                {
                    bst.fireSpeed += scrollY * sensitivity;
                } else if (Input.GetKey(KeyCode.Alpha3))
                {
                    bst.fireAngle += scrollY;
                }
            }
        }

        if(osm.selectedObject != null){
            if(osm.selectedObject.TryGetComponent<SpawnerTile>(out SpawnerTile st))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    st.isNewPlayer = !st.isNewPlayer;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    st.prefabToSpawn = st.isNewPlayer ? playerPrefab : selectedPrefab; 
                }
            }

            // Toggle grid snapping when pressing the 1 key
            if (Input.GetKeyDown(KeyCode.G) && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
            {
                isGridSnapping = !isGridSnapping;
                Debug.Log("Grid Snapping: " + (isGridSnapping ? "Enabled" : "Disabled"));
            }
        }

        
    }

    // Populate the dropdown with the available prefabs
    void PopulatePrefabDropdown()
    {
        prefabDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (GameObject prefab in prefabList)
        {
            options.Add(prefab.name);
        }

        prefabDropdown.AddOptions(options);
        prefabDropdown.onValueChanged.AddListener(delegate { OnPrefabSelected(prefabDropdown.value); });

        // Default to the first prefab in the list
        OnPrefabSelected(0);
    }

    // Called when the player selects a prefab from the dropdown
    void OnPrefabSelected(int index)
    {
        selectedPrefab = prefabList[index];
        Debug.Log("Selected Prefab: " + selectedPrefab.name);
    }

    // Place the currently selected prefab at the clicked location
void PlaceObject()
{
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePosition.z = 0; // Set Z to 0 since it's 2D

    Vector3 positionToPlace = mousePosition;

    // Apply grid snapping if enabled
    if (isGridSnapping)
    {
        positionToPlace = new Vector3(
            Mathf.Round(positionToPlace.x / gridSize) * gridSize,
            Mathf.Round(positionToPlace.y / gridSize) * gridSize,
            0 // Z is 0 for 2D
        );
    }

    // Instantiate the selected prefab without inheriting the parent's scale
    GameObject newObject = Instantiate(selectedPrefab, positionToPlace, Quaternion.identity);

    // Set the correct local scale after instantiation (ensure it retains original scale)
    newObject.transform.localScale = selectedPrefab.transform.localScale;

    // Now parent the object to the objectParent, but without affecting its scale
    newObject.transform.SetParent(objectParent, true); 
}

}
