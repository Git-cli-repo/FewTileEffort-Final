using UnityEngine;

public class SkinManager : MonoBehaviour
{
    // Key for storing the skin index in PlayerPrefs
    private string skinKey = "skin";

    // Method to change the skin index and save it to PlayerPrefs
    public void ChangeSkin(int skinIndex)
    {
        // Save the selected skin index to PlayerPrefs as a number
        PlayerPrefs.SetInt(skinKey, skinIndex);
        PlayerPrefs.Save();

        Debug.Log("Skin changed and saved as: " + skinIndex);
    }
}
