using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUnlocker : MonoBehaviour
{
    public string[] requiredBadges;
    public string levelToUnlock;

    public void TryUnlockLevel()
    {
        foreach (string badge in requiredBadges)
        {
            if (!BatMan.instance.HasBadge(badge))
            {
                Debug.Log("Level Locked: Missing Badge - " + badge);
                return;
            }
        }
        Debug.Log("Level Unlocked: " + levelToUnlock);
        SceneManager.LoadScene(levelToUnlock);
    }
}
