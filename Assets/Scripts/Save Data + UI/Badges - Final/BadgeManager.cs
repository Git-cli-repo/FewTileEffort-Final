using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BadgeManager : MonoBehaviour
{
    public List<Badge> badges = new List<Badge>();
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "badges.json");
        LoadBadges();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.R)){
            ResetBadges();
        }
    }

    public void UnlockBadge(string badgeID)
    {
        Badge badge = badges.Find(b => b.badgeID == badgeID);
        if (badge != null && !badge.isUnlocked)
        {
            badge.isUnlocked = true;
            SaveBadges();
        }
    }

    public void SaveBadges()
    {
        string json = JsonUtility.ToJson(new BadgeListWrapper { badges = this.badges }, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void ResetBadges()
    {
        foreach (Badge badge in badges)
        {
            badge.isUnlocked = false;
        }
        SaveBadges();
        Debug.Log("All badges have been reset.");
    }

    public void LoadBadges()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            BadgeListWrapper wrapper = JsonUtility.FromJson<BadgeListWrapper>(json);
            badges = wrapper.badges;
        }
        else
        {
            InitializeDefaultBadges();
            SaveBadges();
        }
    }

    private void InitializeDefaultBadges()
    {
        badges.Add(new Badge { badgeID = "weAllStartSomewhere", badgeName = "We All Start Somewhere", badgeDescription = "Complete the Main Game", isUnlocked = false });
        badges.Add(new Badge { badgeID = "bittersweet", badgeName = "Bittersweet", badgeDescription = "Complete the Epilogue", isUnlocked = false });
        badges.Add(new Badge { badgeID = "timingsGalore", badgeName = "Timings Galore", badgeDescription = "Complete Hardcore Mode", isUnlocked = false });
        badges.Add(new Badge { badgeID = "nowhereButUp", badgeName = "Nowhere but Up", badgeDescription = "Complete The Trials", isUnlocked = false });
        badges.Add(new Badge { badgeID = "newFeatures", badgeName = "New Features?", badgeDescription = "Complete The Gauntlets", isUnlocked = false });
        badges.Add(new Badge { badgeID = "gettingSomewhere", badgeName = "Getting Somewhere", badgeDescription = "Complete the Challenge Zone", isUnlocked = false });
        badges.Add(new Badge { badgeID = "speedDemon", badgeName = "Speed Demon", badgeDescription = "Complete Main Game in under 10 Minutes", isUnlocked = false });
        badges.Add(new Badge { badgeID = "startOfHard", badgeName = "Start of Hard", badgeDescription = "Complete Main Game completionist", isUnlocked = false });
        badges.Add(new Badge { badgeID = "tooHard4U", badgeName = "Too Hard 4 U", badgeDescription = "Complete Challenge Zone completionist", isUnlocked = false });
        badges.Add(new Badge { badgeID = "notAgain", badgeName = "Not Again...", badgeDescription = "Beat Epilogue Completionist", isUnlocked = false });
        badges.Add(new Badge { badgeID = "superHardcore", badgeName = "Super Hardcore?!", badgeDescription = "Wait for the update :)", isUnlocked = false });
        badges.Add(new Badge { badgeID = "gottaGoFast", badgeName = "Gotta Go Fast", badgeDescription = "Complete Hardcore in Under 10 Minutes", isUnlocked = false });
        badges.Add(new Badge { badgeID = "expertPlusPlus", badgeName = "Expert++", badgeDescription = "Complete Hardcore Completionist", isUnlocked = false });
        badges.Add(new Badge { badgeID = "theTrueEnding", badgeName = "The True Ending", badgeDescription = "Wait for the update :) Complete All in One Completionist", isUnlocked = false });
    }

    [System.Serializable]
    private class BadgeListWrapper
    {
        public List<Badge> badges;
    }
}
