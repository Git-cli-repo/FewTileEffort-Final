using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathCMode : MonoBehaviour
{
    public GameObject particleOnDeath;
    public GameObject player;
    public string deathScene;
    public bool isCompletionist = true;
    public GameObject fragmentPrefab; // Assign the Fragment prefab in the inspector
    public int fragmentsCount = 5; // Number of fragments
    public float explosionForce = 500f; // Force of the explosion
    public float winForce = 2.0f;
    private SpriteRenderer spriteRenderer;
    public AudioSource deathSFXObject;
    public bool isInvincible = false;
    public GameObject[] lavaTiles;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the player object.");
        }
        if(isInvincible){
            lavaTiles = GameObject.FindGameObjectsWithTag("Lava");
            foreach(GameObject gob in lavaTiles){
                Destroy(gob.GetComponent<BoxCollider2D>());
            }
        }
    }

    // Function to hide the player by setting the alpha to 0
    private void HidePlayer()
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }
    }

    // Function to show the player by setting the alpha to 1
    public void ShowPlayer()
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }
    }

    // Function to trigger the death explosion
    public void TriggerExplosion()
    {
        for (int i = 0; i < fragmentsCount; i++)
        {
            SpawnFragment(explosionForce);        
            
        }

        HidePlayer();// Disable the player object
    }

    // Function to trigger the win animation
    public void TriggerCelebration()
    {
        for (int i = 0; i < fragmentsCount; i++)
        {
            SpawnFragment(winForce);
        }
    }


    // Function to spawn and apply force to a fragment
    private void SpawnFragment(float force)
    {
        GameObject fragment = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
        SpriteRenderer fragmentSprite = fragment.GetComponent<SpriteRenderer>();
        Color newColor;
        string hexCode = AchievementManager.Instance.ReadFromFile("tracker.json", "playerColor", true);
        if (UnityEngine.ColorUtility.TryParseHtmlString("#" + hexCode, out newColor))
        {
            fragmentSprite.material.color = newColor;
        }

        if (rb != null)
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            rb.AddForce(direction * Mathf.Pow(force, 2), ForceMode2D.Impulse);
        }
    }

    // Function to wait for a specified amount of time
    public void WaitToDie(float time)
    {
        StartCoroutine(WaitCoroutine(time));
    }

    // Coroutine to handle the wait
    private IEnumerator WaitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        GameOver();
    }

    public void WaitToWin(float time)
    {
        StartCoroutine(WinCoroutine(time));
    }

    private IEnumerator WinCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        NextLevel();
    }


    private void OnCollisionEnter2D(Collision2D collision){
        if(!isInvincible){
            if(collision.gameObject.CompareTag("Lava") || collision.gameObject.CompareTag("LavaOn") || collision.gameObject.CompareTag("BigLava")){
                TriggerExplosion();
                WaitToDie(0.5f);
                
            } else if(collision.gameObject.CompareTag("Gorilla")){
                WaitToWin(0.5f);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision){
    
    }
    
    private void OnCollisionExit2D(Collision2D collision){

    }

    private void GameOver(){
        if(isCompletionist){
            PlayerPrefs.SetInt("PlayerDeaths", PlayerPrefs.GetInt("PlayerDeaths", 0) + 1);
            SceneManager.LoadScene(deathScene);
            CompleteAchievementsRunManager.Instance.OnPlayerDeath();
        
        } else if(!isCompletionist){
            PlayerPrefs.SetInt("PlayerDeaths", PlayerPrefs.GetInt("PlayerDeaths", 0) + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            CompleteAchievementsRunManager.Instance.OnPlayerDeath();
        }
    }
    private void NextLevel(){
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) + 1);
    }
}
