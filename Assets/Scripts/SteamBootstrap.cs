using UnityEngine;
using Steamworks;
using Microsoft.SqlServer.Server;

public class SteamBootstrap : MonoBehaviour
{
    public static bool Initialized { get; private set; }
    public Callback<UserStatsReceived_t> userStatsRecived { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (!SteamAPI.Init())
        {
            Debug.LogError("Steam Init Failed");
            return;
        }

        Initialized = true;
        userStatsRecived = Callback<UserStatsReceived_t>.Create(OnUserStatsRecieved);

        // Give the API 1 second to settle before trying to write data
        StartCoroutine(UnlockRoutine());
    }

    System.Collections.IEnumerator UnlockRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        bool success = SteamUserStats.SetAchievement("WE_ALL_START_SOMEWHERE");
        bool stored = SteamUserStats.StoreStats();
        
        Debug.Log($"[Attempt] Achievement: {success}, Stored: {stored}");

        if (!success)
        {
            // If this is STILL false, Steam literally doesn't see the name
            Debug.LogError("Steam does not recognize the Achievement API Name. Check if Published!");
        }

        foreach(AchievementInfo ach in AchievementManager.Instance.GetAllAchievements())
        {
            if (AchievementManager.Instance.IsUnlocked(ach.achievementID))
            {
                bool res = SteamUserStats.GetAchievement(ach.achievementID, out bool t);
                if (res)
                {
                    if (!t)
                    {
                       bool res2 = SteamUserStats.SetAchievement(ach.achievementID);
                       bool stored2 = SteamUserStats.StoreStats();
                        if (res2 && stored2)
                        {
                            Debug.LogError($"Set achievement ${ach.achievementID}");
                        } else
                        {
                            Debug.LogError($"Failed to set achievement {ach.achievementID}");
                        }
                    }
                } else
                {
                    Debug.LogError($"Steam does not recognize the Achievement API Name for achivement {ach.achievementID}. Check if Published!");
                }
            }   
        }
    }    
    public void OnUserStatsRecieved(UserStatsReceived_t uStats)
    {
        if (uStats.m_eResult != EResult.k_EResultOK)
        {
            // Not yet
            Debug.LogError("Not Yet");
            return;
        } else
        {
            if (SteamAPI.Init())
            {
            }
        }
    }


    void Update()
    {
        if (Initialized)
            SteamAPI.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        if (Initialized)
            SteamAPI.Shutdown(); // test
    }
}
