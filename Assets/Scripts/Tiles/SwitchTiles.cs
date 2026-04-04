using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchTiles : MonoBehaviour
{
    public GameObject[] blueArray;
    public GameObject[] redArray;
    public string switchval;
    public bool isSet = false;
    public void Start()
    {
        int i = 0;

        if (!isSet)
        {
            redArray = GameObject.FindGameObjectsWithTag("SwitchR");
            blueArray = GameObject.FindGameObjectsWithTag("SwitchB");
        }

        foreach (GameObject gob in blueArray)
        {
            // Get the Transform component from the object we're reading from
            Transform t = gob.transform;

            // Check if the GameObject has at least one child
            if (t.childCount > 0)
            {
                // Get the first child of the GameObject
                Transform firstChild = t.GetChild(0);

                // Detach the first child by setting its parent to null
                firstChild.SetParent(null);

                if (t.childCount == 0)
                {
                    Debug.LogWarning("Successfully deparented the Outline object of " + "Switch Tile " + i + ", " + gob.name);
                }
                else
                {
                    Debug.LogWarning("Could not successfully deparent the Outline object of " + "Switch Tile " + i + ", " + gob.name);
                }
            }
            else
            {
                // Send message to console if object has no children
                Debug.Log("Switch Tile " + i + ", " + gob.name + " has no Child GameObjects");
            }

            // Increment the counter
            i++;
        }

        foreach (GameObject gob in redArray)
        {
            // Get the Transform component from the object we're reading from
            Transform t = gob.transform;

            // Check if the GameObject has at least one child
            if (t.childCount > 0)
            {
                // Get the first child of the GameObject
                Transform firstChild = t.GetChild(0);

                // Detach the first child by setting its parent to null
                firstChild.SetParent(null);
            }
        }

        SetState("Red");
    }
    public void Update(){
        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.switchTilesKey)){   
                SetState(switchval);
        }   
    }

    public void SetState(string key){
        if(key == "Blue"){
            foreach(GameObject gob in blueArray){
                gob.SetActive(true);
            }
            foreach(GameObject gob in redArray){
                gob.SetActive(false);
            }
            switchval = "Red";
        } else if(key == "Red"){
            foreach(GameObject gob in blueArray){
                gob.SetActive(false);
            }
            foreach(GameObject gob in redArray){
                gob.SetActive(true);
            }
            switchval = "Blue";
        }
    }
}
