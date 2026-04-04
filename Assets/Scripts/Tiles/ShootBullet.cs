using System.Collections;
using UnityEngine;

public class FiringTile : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireAngle = 45f;
    public float fireSpeed = 10f;
    public float fireRate = 2f;
    public float fireOffset = 0f;
    private bool isFirst = false;

    private void Start()
    {
        if(gameObject.activeSelf == true){
            StartCoroutine(WaitTime());
        }
    }

    public IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(fireOffset);
        InvokeRepeating(nameof(FireBullet), 0f, fireRate);
    }

    private void FireBullet()
    {
        // Calculate direction vector from the fireAngle, ensuring it's a global direction
        Vector2 fireDirection = Quaternion.Euler(0, 0, fireAngle) * Vector2.right;

        // Instantiate the bullet at the tile's position with rotation aligned to the fireDirection
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, fireAngle));

        // Assuming the bullet prefab has a Rigidbody2D component
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Set the Rigidbody2D to Kinematic
;            rb.bodyType = RigidbodyType2D.Kinematic;

            // Apply velocity to the bullet to move it in the fireDirection at the specified speed
            // This ensures the bullet moves forward in the direction specified by fireAngle
            rb.linearVelocity = fireDirection.normalized * fireSpeed;
        }
    }

    public void RestartFiring()
    {
        CancelInvoke(nameof(FireBullet));
        InvokeRepeating(nameof(FireBullet), 0f, fireRate);
    }


}
