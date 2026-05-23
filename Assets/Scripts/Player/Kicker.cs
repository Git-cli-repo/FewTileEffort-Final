using UnityEngine;
using System.Collections;

public class Kicker : MonoBehaviour
{
    public GameObject player;
    public LineRenderer lineRenderer;
    public GameObject hookPrefab;
    private Vector3 targetPoint;
    private bool isGrappling = false;
    private bool isSwinging = false;
    private bool isDescending = false;
    private float swingStartTime;
    public float grappleSpeed = 5f;
    public float swingSpeed = 2f;
    public float maxSwingArc = 60f;
    private float initialSwingDistance;
    private PlayerMovement playerMovement;
    public float swingStartDistance = 2f;
    public float damping = 0.98f; // Damping factor to decrease speed and amplitude

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
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

        if (isSwinging)
        {
            Swing();
        }

        if (isDescending)
        {
            ContinueDescent();
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
        hookPrefab.transform.position = point;
        initialSwingDistance = Vector3.Distance(transform.position, point);
    }

    void StopGrappling()
    {
        isGrappling = false;
        isSwinging = false;
        isDescending = false;
        lineRenderer.enabled = false;
        playerMovement.body.gravityScale = 2.5f;
    }

    void GrappleToTarget()
    {
        playerMovement.body.gravityScale = 0f;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * grappleSpeed);

        lineRenderer.SetPosition(0, transform.position);

        if (Vector3.Distance(transform.position, targetPoint) <= swingStartDistance)
        {
            StartSwinging();
        }
    }

    void StartSwinging()
    {
        isGrappling = false;
        isSwinging = true;
        swingStartTime = Time.time;
    }

    void Swing()
    {
        float elapsedTime = Time.time - swingStartTime;
        float angle = Mathf.Sin(elapsedTime * swingSpeed) * maxSwingArc;
        float currentDistance = Mathf.Cos(angle * Mathf.Deg2Rad) * initialSwingDistance;
        Vector3 direction = (player.transform.position - targetPoint).normalized;
        Vector3 swingPosition = targetPoint + direction * currentDistance;
        player.transform.position = swingPosition;
        lineRenderer.SetPosition(0, transform.position);
    }

    void StartDescent()
    {
        isSwinging = false;
        isDescending = true;
        playerMovement.body.gravityScale = 0.5f; // Slightly reintroduce gravity for a natural descent
        StartCoroutine(ResetGravity()); // Start coroutine to reset gravity back to normal after a delay
    }

    IEnumerator ResetGravity()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before resetting gravity
        playerMovement.body.gravityScale = 2.5f; // Reset gravity to its normal value
    }

    void ContinueDescent()
    {
        swingSpeed *= damping; // Apply damping to decrease speed
        if (swingSpeed < 0.01f)
        {
            StopGrappling(); // Stop when speed is negligible
            return;
        }

        float angle = Mathf.Sin(Time.time * swingSpeed) * maxSwingArc;
        float currentDistance = Mathf.Cos(angle * Mathf.Deg2Rad) * initialSwingDistance;
        Vector3 direction = (player.transform.position - targetPoint).normalized;
        Vector3 swingPosition = targetPoint + direction * currentDistance;
        player.transform.position = swingPosition;
        lineRenderer.SetPosition(0, transform.position);
    }
}
