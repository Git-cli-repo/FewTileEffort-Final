using UnityEngine;

public class DestroyAllLavaBulletsWithCooldown : MonoBehaviour
{
    public float cooldownDuration = 6.25f; // Cooldown duration in seconds
    private float cooldownTimer = 0f; // Tracks the time until the next allowed destruction

    void Update()
    {
        // Update the cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Check if the 'R' key is pressed and if the cooldown has elapsed
        if (Input.GetKeyDown(KeyCode.R) && cooldownTimer <= 0)
        {
            // Reset the cooldown timer
            cooldownTimer = cooldownDuration;

            // Find all GameObjects in the scene
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            // Loop through each GameObject and destroy those named "Lava Bullet"
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Lava Bullet")) // Adjust name check as needed
                {
                    Destroy(obj);
                }
            }
        }
    }
}
