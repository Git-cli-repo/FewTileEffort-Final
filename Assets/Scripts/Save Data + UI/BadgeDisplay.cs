using UnityEngine;
using UnityEngine.UI;

public class BadgeDisplay : MonoBehaviour
{
    public GameObject badgeIconPrefab;
    public Transform badgeGrid;

    private void Start()
    {
        DisplayBadges();
    }

    private void DisplayBadges()
    {
        // Example badge names
        string[] allBadges = { "Badge1", "Badge2", "Badge3", "Badge4", "Badge5" };

        foreach (string badge in allBadges)
        {
            if (BatMan.instance.HasBadge(badge))
            {
                GameObject badgeIcon = Instantiate(badgeIconPrefab, badgeGrid);
                // Assuming badgeIconPrefab has a child Image component to set the badge icon
                badgeIcon.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Badges/" + badge);
            }
        }
    }
}
