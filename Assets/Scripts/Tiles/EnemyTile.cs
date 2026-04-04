using UnityEngine;

public class EnemyTile : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float moveSpeed = 2f; // Movement speed of the enemy cube

    void Update()
    {
        // Check if the player exists to avoid errors if the player is not in the scene
        if (player != null)
        {
            // Create a new position for the enemy, copying its current position
            Vector3 newPosition = transform.position;
            
            // Update only the x-component of the position to follow the player's x, maintaining its own y and z
            newPosition.x = Mathf.MoveTowards(transform.position.x, player.position.x, moveSpeed * Time.deltaTime);
            
            // Apply the updated position to the enemy
            transform.position = newPosition;
        }
    }
}
