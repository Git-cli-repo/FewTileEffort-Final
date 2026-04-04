using System;
using UnityEngine;

public class GrappleSwing : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleRange = 10f;     // How far you can grapple
    public float swingForce = 10f;       // Initial force toward the grapple point
    public float releaseForce = 5f;      // Force applied upon releasing

    [Header("Layers")]
    public LayerMask grappleLayer;       // Layer of grappleable objects (optional)

    private Rigidbody2D playerRb;
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;

    void Start()
    {
        // Get references
        playerRb = GetComponent<Rigidbody2D>();

        // Add a DistanceJoint2D at runtime
        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;

        // Get or add a LineRenderer to visualize the grapple line
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // Press Q to try grappling
        if (Input.GetKeyDown(CompleteAchievementsRunManager.Instance.grappleKey))
        {
            AttemptGrapple();
            Console.WriteLine("Grapple Started (attempt)");
        }

        // Release grapple on Q up
        if (Input.GetKeyUp(CompleteAchievementsRunManager.Instance.grappleKey))
        {
            ReleaseGrapple();
            Console.WriteLine("Grapple Stopped");
        }

        // Update line renderer positions if grappling
        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    /// <summary>
    /// Attempts to grapple to the nearest valid target in range.
    /// </summary>
    private void AttemptGrapple()
    {
        // Find all colliders within grappleRange of the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grappleRange, grappleLayer);

        if (hits.Length == 0)
        {
            Console.WriteLine("Grapple Failed! No GrapplePoints in range!");
            // No grapple targets in range
            return;
        }


        // Pick the closest valid grapple object
        Collider2D closestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D col in hits)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = col;
            }
        }

        // If we found a valid target
        if (closestTarget != null)
        {
            Console.WriteLine("Grapple Success, found in point " + closestTarget.gameObject.transform.position + ".");
            
            // Determine the exact point to connect the DistanceJoint (closest point on the collider)
            grapplePoint = closestTarget.ClosestPoint(transform.position);

            // Enable the DistanceJoint2D
            distanceJoint.enabled = true;
            distanceJoint.connectedAnchor = grapplePoint;
            distanceJoint.distance = Vector2.Distance(transform.position, grapplePoint);

            // Enable line renderer
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            // Apply an initial force to "pull" or "swing" the player toward the grapple point
            Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
            playerRb.AddForce(direction * swingForce, ForceMode2D.Impulse);

            //Add one to the RunManager's Grapple Count towards the achievement
            CompleteAchievementsRunManager.Instance.OnGrappleSuccess();
        }
    }

    private void ReleaseGrapple()
    {
        Console.WriteLine("Grapple Ended");
        if (distanceJoint.enabled)
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;

            // Optionally add a release force in the direction from player to grapplePoint
            Vector2 releaseDirection = (grapplePoint - (Vector2)transform.position).normalized;
            playerRb.AddForce(releaseDirection * releaseForce, ForceMode2D.Impulse);
        }
    }

    // For debug: visualize the OverlapCircle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
    }
}
