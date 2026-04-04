using UnityEngine;
using UnityEngine.UI;

public class BadgeUI : MonoBehaviour
{
    public Color unlockedColor = Color.green; // The color to show when a badge is unlocked
    public Color lockedColor = Color.white; // The color to show when a badge is locked

    private BadgeManager badgeManager;

    private void Start()
    {
        badgeManager = FindObjectOfType<BadgeManager>();
        if (badgeManager == null)
        {
            Debug.LogError("BadgeManager not found in the scene.");
            return;
        }
        DisplayBadges();
    }

    private void DisplayBadges()
    {
        foreach (Badge badge in badgeManager.badges)
        {
            GameObject badgeObject = GameObject.Find(badge.badgeID);
            if (badgeObject == null)
            {
                Debug.LogError($"Badge GameObject with name {badge.badgeID} not found.");
                continue;
            }

            Image badgeImage = badgeObject.GetComponent<Image>();
            if (badgeImage == null)
            {
                Debug.LogError($"Image component not found on Badge GameObject with name {badge.badgeID}.");
                continue;
            }

            badgeImage.color = badge.isUnlocked ? unlockedColor : lockedColor;
            Debug.Log($"Badge {badge.badgeID} set to color {(badge.isUnlocked ? "unlocked" : "locked")}.");
        }
    }
}
