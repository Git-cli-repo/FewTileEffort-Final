using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject player;
    public LineRenderer lineRenderer;
    public GameObject hookPrefab;
    private Vector3 targetPoint;
    private bool isGrappling = false;
    private bool isSwinging = false;
    private Rigidbody2D playerRb;
    private float swingRadius;
    public float grappleForce = 10f;
    public float swingForce = 5f;
    public float damping = 0.98f; // Damping factor to decrease speed and amplitude
    public float lineWidth = 0.35f;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                targetPoint = hit.point;
                StartGrappling(targetPoint);
            }
        }

        if (isGrappling)
        {
            GrappleToTarget();
        }

        if (isSwinging && Input.GetKeyDown(KeyCode.F))
        {
            StartDescent();
        }
    }

    void StartGrappling(Vector3 point)
    {
        isGrappling = true;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, point);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        hookPrefab.transform.position = point;
        swingRadius = 1.4f;// Vector3.Distance(transform.position, point) * 0.1f;
    }

    void GrappleToTarget()
    {
        Vector2 direction = (targetPoint - transform.position).normalized;
        playerRb.AddForce(direction * grappleForce);

        lineRenderer.SetPosition(0, transform.position);

        if (Vector3.Distance(transform.position, targetPoint) <= swingRadius)
        {
            //playerRb.velocity = Vector2.zero; // Stop the player's movement when they reach the grappling point
            isGrappling = false;
            isSwinging = true;
        }
    }

    void FixedUpdate()
    {
        if (isSwinging)
        {
            Swing();
        }
    }

    void Swing()
    {
        Vector2 directionToHook = (targetPoint - transform.position).normalized;
        Vector2 perpDirection = Vector2.Perpendicular(directionToHook).normalized;
        playerRb.AddForce(perpDirection * swingForce);
        
        // Apply damping to gradually reduce the swinging motion
        playerRb.linearVelocity *= damping;

        lineRenderer.SetPosition(0, player.transform.position);
    }

    void StartDescent()
    {
        isSwinging = false;
        // Optionally, you can apply a downward force to simulate a faster descent or let gravity take its natural course.
    }
}
