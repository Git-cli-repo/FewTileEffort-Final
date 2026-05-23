using UnityEngine;

public class BadgeTrigger : MonoBehaviour
{
    public string badgeID; // The ID of the badge to unlock

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PlayerObj"))
        {
            BadgeManager badgeManager = FindObjectOfType<BadgeManager>();
            if (badgeManager != null)
            {
                badgeManager.UnlockBadge(badgeID);
                Debug.Log("Unlocked " + badgeID);
            }
        }
    }
}
