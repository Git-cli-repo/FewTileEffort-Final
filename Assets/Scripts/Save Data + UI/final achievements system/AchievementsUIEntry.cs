using UnityEngine;
using UnityEngine.UI;
using TMPro; // or use regular Text if you prefer
using UnityEngine.SceneManagement;

public class AchievementUIEntry : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image lockOverlay; // e.g. a semi-transparent lock icon

    private string achievementID;

    public void Setup(AchievementData data)
    {
        achievementID = data.achievementID;
        if (iconImage) iconImage.sprite = data.icon;
        if (titleText) titleText.text = data.title;
        if (descriptionText) descriptionText.text = data.description;

        RefreshLockState();
    }

    public void RefreshLockState()
    {
        bool unlocked = AchievementManager.Instance.IsUnlocked(achievementID);
        if (lockOverlay) lockOverlay.gameObject.SetActive(!unlocked);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("Main Menu");
        }
    }
}
