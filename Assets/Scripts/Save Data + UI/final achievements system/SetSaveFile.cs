using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
public class SetSaveFile : MonoBehaviour
{
    public TMP_InputField createSaveFile;
    public UnityEngine.UI.Toggle copySaveFile;
    public TMP_InputField saveFileToCopy;
    public TMP_InputField setSaveFile;

    public void CreateNewSaveFile()
    {
        AchievementManager.Instance.CreateNewSaveFile(createSaveFile.text, copySaveFile.enabled, saveFileToCopy.text);
    }

    public void ChangeSaveFile()
    {
        AchievementManager.Instance.SetSaveFile(setSaveFile.text);
    }
}