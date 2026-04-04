using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // For reloading the scene
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using SFB; // Standalone File Browser
using TMPro;

class LevelLoaaad: MonoBehaviour {
    public string sceneName;
    public void LoadScene(){
        SceneManager.LoadScene(sceneName);
    }
}