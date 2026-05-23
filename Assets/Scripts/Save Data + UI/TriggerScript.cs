using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("PlayerObj")) // Ensure your player GameObject is tagged as "Player"
    {
        // Assuming this script is attached to the trigger object and has access to a SaveData instance
        SaveData saveData = FindObjectOfType<SaveData>(); // This finds the SaveData component in the scene
        if (saveData != null)
        {
            // Call OnSaveTrigger with the current values stored in saveData.data
            saveData.OnSaveTrigger(
                saveData.data.latestLevel,
                saveData.data.mainGameCompleted,
                saveData.data.unlockHardcore,
                saveData.data.hardcoreCompleted,
                saveData.data.unlockChallengeZone,
                saveData.data.challengeZoneCompleted,
                saveData.data.unlockGauntlets,
                saveData.data.gauntletsCompleted,
                saveData.data.unlockTheTrials,
                saveData.data.theTrialsCompleted,
                saveData.data.unlockEpilogue,
                saveData.data.epilogueCompleted,
                saveData.data.unlockSuperHardcore,
                saveData.data.superHardcoreCompleted,
                saveData.data.unlockEndGame,
                saveData.data.endGameCompleted
            );
            Debug.Log("Game Saved");
            Debug.Log(saveData.data.latestLevel);
        }
    }
}

}
