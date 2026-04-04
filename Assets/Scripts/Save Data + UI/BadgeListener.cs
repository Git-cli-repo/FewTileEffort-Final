using UnityEngine;

public class PlayerPrefActivator : MonoBehaviour
{
    public string playerPrefKey = "examplePref";

    void OnTriggerEnter(Collider gob)
    {
        if (gob.CompareTag("PlayerObj"))
        {
            PlayerPrefs.SetInt(playerPrefKey, 1); // Set the PlayerPref to true (1)
            PlayerPrefs.Save();
        }
    }
}
