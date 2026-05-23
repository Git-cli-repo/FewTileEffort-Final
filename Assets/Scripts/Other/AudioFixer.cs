using UnityEngine;

public class PersistentAudioPlayer : MonoBehaviour
{
    private static PersistentAudioPlayer instance = null; // Singleton instance
    public bool isenabled = false;

    void Awake()
    {
       if(isenabled){
        // Check if an instance already exists
        if (instance == null)
        {
            // If no instance exists, this becomes the singleton instance
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this GameObject from being destroyed on scene loads
        }
        else if (instance != this)
        {
            // If an instance already exists and it's not this one, destroy this to enforce the singleton pattern
            Destroy(gameObject);
        }
    }
 }
}
