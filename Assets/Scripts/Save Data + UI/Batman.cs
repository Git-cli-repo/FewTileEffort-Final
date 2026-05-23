using UnityEngine;

public class BatMan : MonoBehaviour
{
    public static BatMan instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AwardBadge(string badgeName)
    {
        PlayerPrefs.SetInt(badgeName, 1);
        PlayerPrefs.Save();
        Debug.Log("Badge Awarded: " + badgeName);
    }

    public bool HasBadge(string badgeName)
    {
        return PlayerPrefs.GetInt(badgeName, 0) == 1;
    }
}
