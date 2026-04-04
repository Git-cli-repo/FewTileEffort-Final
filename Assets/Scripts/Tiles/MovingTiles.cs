using System.Linq;
using UnityEngine;

public class MovingTile : MonoBehaviour
{
    public GameObject[] waypoints; // Array of waypoint GameObjects
    public float moveSpeed = 2f; // Speed at which the tile moves
    private int waypointIndex = 0; // Current waypoint index
    private Vector3 lastPosition;
    private Vector3 frameDelta;

    void Start()
    {
        lastPosition = transform.position;
    }


    void Update()
    {
        if(waypoints.Count() == 0) waypoints = new GameObject[2];
        MoveBetweenWaypoints();
    }

    void MoveBetweenWaypoints()
    {
        if (waypointIndex >= waypoints.Length) return; // Check if all waypoints are covered

        // Move the tile towards the current waypoint using Vector2 for 2D movement
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);

        // Check if the tile has reached the current waypoint using Vector2.Distance for 2D
        if (Vector2.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.1f)
        {
            waypointIndex++; // Move to the next waypoint
            if (waypointIndex >= waypoints.Length)
            {
                waypointIndex = 0; // Target the first waypoint
            }
        }
    }
}

//Mango#6417