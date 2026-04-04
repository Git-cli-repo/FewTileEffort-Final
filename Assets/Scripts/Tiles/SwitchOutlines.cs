using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOutlines : MonoBehaviour
{
    public GameObject[] blueArray;
    public GameObject[] redArray;
    public string switchval;
    public bool isSet = false;
    public void Start(){
        if(!isSet){
            redArray = GameObject.FindGameObjectsWithTag("OSwitchR");
            blueArray = GameObject.FindGameObjectsWithTag("OSwitchB");
            SetState("Red");
        }
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