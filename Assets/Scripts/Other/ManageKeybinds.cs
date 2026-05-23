using UnityEngine; 
using TMPro;

public class ManageKeybinds : MonoBehaviour
{
    public TMP_Dropdown keybindDropdown;
    public KeyCode selectedKey;
    public string keybindToChange;

    void Start()
    {
        keybindDropdown = GetComponent<TMP_Dropdown>();
    }

    public void OnKeybindChanged(int index)
    {
        AchievementManager.Instance.WriteToFile("tracker.json", keybindDropdown.options[index].text, keybindToChange);
    }
}