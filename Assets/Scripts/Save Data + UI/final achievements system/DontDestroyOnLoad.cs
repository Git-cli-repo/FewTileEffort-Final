using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DontDestroy: MonoBehaviour{
    void Start(){
        if (gameObject.name == "PersistentUI")
        {
            if (GameObject.FindGameObjectsWithTag("Speedrun Timer").Length != 1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        { 
            DontDestroyOnLoad(this.gameObject);
        }
        
    }
}