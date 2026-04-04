using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
public class SpawnerTile: MonoBehaviour {
    public string objectTag;
    public bool isSpawned = false;
    public bool isNewPlayer;
    public GameObject oldPlayer;
    
    public CinemachineVirtualCamera virtualCamera;
    public GameObject spawnAt;
    public GameObject prefabToSpawn;
    void Start(){

    }
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("PlayerObj")){
            if(isSpawned == false){
                GameObject spawnedObject = Instantiate(prefabToSpawn, spawnAt.gameObject.transform.position, spawnAt.transform.rotation);
                spawnAt.gameObject.SetActive(false);
                isSpawned = true;
                if(isNewPlayer == true){
                    // Grab components for making sure Switch and Dark tiles don't break
                    DarkLightTiles newDarkLight = spawnedObject.GetComponent<DarkLightTiles>();
                    SwitchTiles newSwitchTiles = spawnedObject.GetComponent<SwitchTiles>();
                    SwitchOutlines newSwitchOutlines = spawnedObject.GetComponent<SwitchOutlines>();
                    DarkLightTiles oldDarkLight = oldPlayer.GetComponent<DarkLightTiles>();
                    SwitchTiles oldSwitchTiles = oldPlayer.GetComponent<SwitchTiles>();
                    SwitchOutlines oldSwitchOutlines = oldPlayer.GetComponent<SwitchOutlines>();

                    // Updating Darklight Values
                    newDarkLight.isSet = true;
                    newDarkLight.DarkTiles = oldDarkLight.DarkTiles;
                    newDarkLight.LightTiles = oldDarkLight.LightTiles;
                    newDarkLight.currentActive = oldDarkLight.currentActive;

                    // Updating Switch Tile Values
                    newSwitchTiles.isSet = true;
                    newSwitchTiles.redArray = oldSwitchTiles.redArray;
                    newSwitchTiles.blueArray = oldSwitchTiles.blueArray;
                    newSwitchTiles.switchval = oldSwitchTiles.switchval;

                    // Updating Switch Outline Values for Switch Tiles
                    newSwitchOutlines.isSet = true;
                    newSwitchOutlines.redArray = oldSwitchOutlines.redArray;
                    newSwitchOutlines.blueArray = oldSwitchOutlines.blueArray;
                    newSwitchOutlines.switchval = oldSwitchOutlines.switchval;

                    // Deactivate the old player
                    collision.gameObject.SetActive(false);
                    
                    // Update the Virtual Camera to target the new player instead
                    virtualCamera.LookAt = spawnedObject.gameObject.transform;
                    virtualCamera.Follow = spawnedObject.gameObject.transform;
                }
            }
        }
    }
}