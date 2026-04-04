using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For using Lists and Dictionaries

public class FirstBoss : MonoBehaviour
{
    public Transform player;
    public GameObject lavaBulletPrefab;
    public GameObject enemyCubePrefab;
    public float hoverHeight = 5f;
    public float moveSpeed = 3f;
    public float shootIntervalPhaseOne = 6f;
    public float shootIntervalPhaseTwo = 3f;
    private float shootTimer;
    private bool isPhaseTwo = false;

    // Positions for wave movement
    public float leftEdgeX;
    public float rightEdgeX;
    public float waveFireRate = 0.5f; // Rate at which bullets are fired during the wave

    private BossHealth bossHealth;

    private List<GameObject> homingBullets = new List<GameObject>(); // Store homing bullets
    private Dictionary<GameObject, Vector3> bulletDirections = new Dictionary<GameObject, Vector3>(); // Store directions for after homing

    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        shootTimer = shootIntervalPhaseOne;
    }

    void Update()
    {
        HoverAbovePlayer();

        if (!isPhaseTwo && bossHealth.currentHealth <= 250)
        {
            isPhaseTwo = true;
            shootTimer = shootIntervalPhaseTwo;
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            if (isPhaseTwo)
            {
                PhaseTwoAttack();
            }
            else
            {
                ShootLavaBullet();
            }

            shootTimer = isPhaseTwo ? shootIntervalPhaseTwo : shootIntervalPhaseOne;
        }

        // Update positions of homing bullets
        for (int i = homingBullets.Count - 1; i >= 0; i--)
        {
            if (homingBullets[i] != null)
            {
                // Homing towards the player
                Vector3 direction = (player.position - homingBullets[i].transform.position).normalized;
                homingBullets[i].transform.position += direction * Time.deltaTime * 10; // Adjust the speed as needed

                // If bullet is in the bulletDirections dictionary and has a non-zero direction, move it straight
                if (bulletDirections.ContainsKey(homingBullets[i]) && bulletDirections[homingBullets[i]] != Vector3.zero)
                {
                    homingBullets[i].transform.position += bulletDirections[homingBullets[i]] * Time.deltaTime * 10;
                }
            }
            else
            {
                // Remove null references (e.g., if the bullet was destroyed)
                homingBullets.RemoveAt(i);
            }
        }
    }

    void HoverAbovePlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + hoverHeight, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void ShootLavaBullet()
    {
        if (player != null)
        {
            GameObject bullet = Instantiate(lavaBulletPrefab, transform.position, Quaternion.identity);
            homingBullets.Add(bullet); // Add bullet to homing list
            bulletDirections.Add(bullet, Vector3.zero); // Initialize direction
            StartCoroutine(StopHoming(bullet, 0.5f)); // Homing for 0.5 seconds
        }
    }

    IEnumerator StopHoming(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bullet != null && homingBullets.Contains(bullet))
        {
            // Calculate the final direction when homing stops
            Vector3 finalDirection = (player.position - bullet.transform.position).normalized;
            bulletDirections[bullet] = finalDirection; // Store the direction

            // Stop homing and let the bullet move straight
            homingBullets.Remove(bullet);
        }
    }

    void PhaseTwoAttack()
    {
        float chance = Random.Range(0f, 1f);

        if (chance < 0.2f)
        {
            StartCoroutine(FireWaveOfBullets());
        }
        else if (chance < 0.6f)
        {
            ShootLavaBullet();
        }
        else if (chance < 0.9f)
        {
            StartCoroutine(ShootTwoHomingBullets());
        }
        else
        {
            Instantiate(enemyCubePrefab, transform.position, Quaternion.identity);
        }
    }

    IEnumerator ShootTwoHomingBullets()
    {
        ShootLavaBullet();
        yield return new WaitForSeconds(0.15f);
        ShootLavaBullet();
    }

    IEnumerator FireWaveOfBullets()
    {
        float startPositionX = leftEdgeX;
        float endPositionX = rightEdgeX;
        Vector3 startPosition = new Vector3(startPositionX, player.position.y + hoverHeight, transform.position.z);
        Vector3 endPosition = new Vector3(endPositionX, player.position.y + hoverHeight, transform.position.z);

        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (transform.position.x < endPositionX - 0.1f)
        {
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            ShootLavaBullet();
            yield return new WaitForSeconds(waveFireRate);
        }
    }
}
