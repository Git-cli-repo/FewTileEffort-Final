using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class PopulateDropdown : MonoBehaviour
{
    public TMP_Dropdown switchTilesDropdown;
    public List<TMP_Dropdown> dropdownsInScene = new List<TMP_Dropdown>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropdownsInScene = FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None).ToList();
        foreach (TMP_Dropdown dropdown in dropdownsInScene)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(KeyCode.GetValues(typeof(KeyCode)).ConvertTo<List<KeyCode>>().Select(code => code.ToString()).ToList());
            foreach(string s in dropdown.options.Select(opt => opt.text))
            {
                Debug.Log(s);
            }
            dropdown.SetValueWithoutNotify(dropdown.options.FindIndex(option => option.text == AchievementManager.Instance.ReadFromFile("tracker.json", dropdown.gameObject.GetComponent<ManageKeybinds>().keybindToChange, true)));
        }

        // testing artifact - likely unneded

        // switchTilesDropdown.SetValueWithoutNotify(switchTilesDropdown.options.FindIndex(option => option.text == AchievementManager.Instance.ReadFromFile("tracker.json", "switchTilesKeybind", true)));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
