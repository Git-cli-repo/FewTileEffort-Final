using UnityEngine;

public class SemiSolidTiles : MonoBehaviour
{
    private PlatformEffector2D effector;
    private float waitTime = 0.1f;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        // Allow the player to drop down by pressing down arrow and jump button
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                waitTime = 0.1f;
                effector.rotationalOffset = 180f; // Flip the effector
            }
        }

        if (waitTime <= 0)
        {
            effector.rotationalOffset = 0; // Reset the effector to allow standing on the platform
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
