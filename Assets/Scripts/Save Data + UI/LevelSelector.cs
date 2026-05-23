using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public bool loadNext = true;
    [SerializeField] private string sceneName = "";
    public bool canLoad = false;
    public Color normalColor;
    public bool loadRecent = false;
    public int recentLevel;
    public string recentLevelRealNotScam;


    public CompleteAchievementsRunManager.GameMode selectedMode = CompleteAchievementsRunManager.GameMode.None;

    public void Start()
    {
        if (loadNext){
            canLoad = true;
        }

        switch (selectedMode)
        {
            case CompleteAchievementsRunManager.GameMode.MainGame:
                canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "mainGameLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.MainGameComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("WE_ALL_START_SOMEWHERE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "mainGameCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Epilogue:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("WE_ALL_START_SOMEWHERE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "epilogueLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.EpilogueComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("BITTERSWEET")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "epilogueCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Hardcore:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("GETTING_SOMEWHERE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "hardcoreLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.HardcoreComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("TIMINGS_GALORE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "hardcoreCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.ChallengeZone:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("NEW_FEATURES")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "challengeZoneLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.ChallengeZoneComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("GETTING_SOMEWHERE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "challengeZoneCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Trials:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("BITTERSWEET")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "trialsLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Gauntlets:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("NOWHERE_BUT_UP")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "gauntletsLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Tower:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("TIMINGS_GALORE")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "towerLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.AllInOne:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("TOWER_CONQUEROR")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "allInOneLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.TowerComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("TOWER_CONQUEROR")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "towerCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.GauntletsComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("NEW_FEATURES")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "gauntletsCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.Rooms:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("TOWER_CONQUEROR")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "roomsLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.RoomsComp:
                Debug.Log("Mode set to " + selectedMode.ToString());
                if (AchievementManager.Instance.IsUnlocked("KING_OF_PAIN")) canLoad = true;
                if(loadRecent){
                    recentLevel = AchievementManager.Instance.ReadFromFile("tracker.json", "roomsCompLastLevel");
                    Debug.Log("" + recentLevel);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.None:
                Debug.Log("Mode set to NONE.");
                canLoad = true;
                if(loadRecent){
                    Debug.Log("Could not set the recentLevel property of " + this.gameObject.name);
                }
                break;
            case CompleteAchievementsRunManager.GameMode.NotAMode:
                Debug.Log("Mode set to NONE.");
                canLoad = true;
                break;
        }

        if(normalColor == new Color(0f, 0f, 0f, 0f)){
            normalColor = new Color(1f, 1f, 1f, 1f);
        }

        if(canLoad)
        {
            Button buttonHolder = this.gameObject.GetComponent<Button>();
            ColorBlock buttonColors = buttonHolder.colors;
            buttonColors.normalColor = normalColor;
            buttonColors.selectedColor = Color.white;
            buttonColors.pressedColor = normalColor;
            buttonHolder.colors = buttonColors;
            CompleteAchievementsRunManager.Instance.unlockedButtons.Add(this.gameObject);
            Debug.Log(this.gameObject.name + "'s final color: " + buttonHolder.colors.normalColor.ToString() + ", added color: " + buttonColors.normalColor.ToString());

        } else if(!canLoad) {
            Button buttonHolder = this.gameObject.GetComponent<Button>();
            ColorBlock buttonColors = buttonHolder.colors;
            buttonColors.normalColor = CompleteAchievementsRunManager.Instance.lockedColor;
            buttonColors.selectedColor = CompleteAchievementsRunManager.Instance.lockedColor;
            buttonColors.pressedColor = CompleteAchievementsRunManager.Instance.lockedColor;
            buttonHolder.colors = buttonColors;
            CompleteAchievementsRunManager.Instance.lockedButtons.Add(this.gameObject);
            Debug.Log(this.gameObject.name + "'s final color: " + buttonHolder.colors.normalColor.ToString() + ", added color: " + CompleteAchievementsRunManager.Instance.lockedColor.ToString());
        }
    }

    // Called by the UI button, for example
    public void OpenScene()
    {
        if(canLoad)
        {
            // 1) If a run manager is present and the selected mode != None, start a run
            if (selectedMode != CompleteAchievementsRunManager.GameMode.None 
                && selectedMode != CompleteAchievementsRunManager.GameMode.NotAMode && CompleteAchievementsRunManager.Instance != null)
            {
                CompleteAchievementsRunManager.Instance.StartRun(selectedMode);
                Debug.Log("Enabled Mode: " + selectedMode.ToString());
            }

            // 2) Load next scene by index or load a named scene
            if (loadNext)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            } else if(loadRecent) 
            {
                SceneManager.LoadScene(recentLevel);
                Debug.Log(recentLevel);
            } else {
                SceneManager.LoadScene(sceneName);
                Debug.Log("Loaded Scene: " + sceneName);
            }
        }
    }
}