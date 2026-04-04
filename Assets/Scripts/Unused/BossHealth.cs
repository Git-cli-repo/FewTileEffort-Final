using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 500;
    public int currentHealth;
    public Slider healthBar;
    public int lavaDamage = 10; // Damage taken per collision with lava
    public GameObject gorillaPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    void OnCollisionEnter2D(Collision2D collision) // Use OnCollisionEnter for 3D
    {
        // Check if the boss collides with lava
        if (collision.gameObject.CompareTag("Lava"))
        {
            TakeDamage(lavaDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log("Boss defeated!");
            // Implement defeat logic here
            //PlayCutscene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
