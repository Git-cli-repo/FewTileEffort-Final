using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string achievementID;   // Unique ID to match in the save system
    public string title;
    public string description;
    public Sprite icon;
}
