using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{
    public GameObject particleOnDeath;
    public GameObject player;
    public string deathScene;
    public bool isCompletionist = false;
    public GameObject fragmentPrefab; // Assign the Fragment prefab in the inspector
    public int fragmentsCount = 5; // Number of fragments
    public float explosionForce = 500f; // Force of the explosion
    public float winForce = 2.0f;
    private SpriteRenderer spriteRenderer;
    public AudioSource deathSFXObject;
    public LevelSaveManager levelSaveManager;
    public bool isInvincible = false;
    public GameObject[] lavaTiles;
    private bool isDying = false;
    private int deathTriggerCount = 0;
    public bool overrideNextLoadedScene = false;
    public int sceneNumberToOverrideTo;
    public List<GameObject> deathParticleList = new List<GameObject>();

    void Start()
    {
        
        if(FindAnyObjectByType<LevelSaveManager>() != null) levelSaveManager = FindAnyObjectByType<LevelSaveManager>();
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
        SpriteRenderer fragmentSprite = fragment.GetComponent<SpriteRenderer>();
        Color newColor;
        string hexCode = AchievementManager.Instance.ReadFromFile("tracker.json", "playerColor", true);
        if (UnityEngine.ColorUtility.TryParseHtmlString("#" + hexCode, out newColor))
        {
            fragmentSprite.material.color = newColor;
        }
        Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            rb.AddForce(direction * Mathf.Pow(force, 2), ForceMode2D.Impulse);
        }
        deathParticleList.Add(fragment);
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
        if (CompleteAchievementsRunManager.Instance.isPlayingModPack)
        {
            levelSaveManager.LoadLevelFromPath(CompleteAchievementsRunManager.Instance.levelPaths[CompleteAchievementsRunManager.Instance.currentLevelIndex]);
            gameObject.transform.position = GameObject.Find("StartPos").transform.position;
            ShowPlayer();
            foreach(GameObject gob in deathParticleList)
            {
                Destroy(gob);
            }
            deathParticleList = new List<GameObject>();
        } else
        {
            GameOver();
        }
    }

    public void WaitToWin(float time)
    {
        StartCoroutine(WinCoroutine(time));
    }

    private IEnumerator WinCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        if (CompleteAchievementsRunManager.Instance.isPlayingModPack)
        {
            if(CompleteAchievementsRunManager.Instance.currentLevelIndex >= CompleteAchievementsRunManager.Instance.numberOfLevels) { NextLevel(); CompleteAchievementsRunManager.Instance.isPlayingModPack = false; }
            else {
                levelSaveManager.LoadLevelFromPath(CompleteAchievementsRunManager.Instance.levelPaths[CompleteAchievementsRunManager.Instance.currentLevelIndex]);
                gameObject.transform.position = GameObject.Find("StartPos").transform.position;
                CompleteAchievementsRunManager.Instance.lvlChangedAlready = false;
            }
        } else
        {
            NextLevel();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision){
        if(!isInvincible && !CompleteAchievementsRunManager.Instance.isPlayingModPack){
            if(collision.gameObject.CompareTag("Lava") || collision.gameObject.CompareTag("LavaOn") || collision.gameObject.CompareTag("BigLava")){
                TriggerExplosion();
                WaitToDie(0.5f);
                
            } else if(collision.gameObject.CompareTag("Gorilla")){
                deathSFXObject.Play();
                WaitToWin(0.5f);
            }
        } else if (!isInvincible && CompleteAchievementsRunManager.Instance.isPlayingModPack)
        {
            if(collision.gameObject.CompareTag("Lava") || collision.gameObject.CompareTag("LavaOn") || collision.gameObject.CompareTag("BigLava")){
                TriggerExplosion();
                WaitToDie(0.5f);
                
            } else if(collision.gameObject.CompareTag("Gorilla")){
                deathSFXObject.Play();
                if(!CompleteAchievementsRunManager.Instance.lvlChangedAlready) CompleteAchievementsRunManager.Instance.currentLevelIndex++;
                CompleteAchievementsRunManager.Instance.lvlChangedAlready = true;
                WaitToWin(0.5f);
                collision.gameObject.SetActive(false);
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
    public void NextLevel(){
        if(overrideNextLoadedScene) SceneManager.LoadScene(sceneNumberToOverrideTo);
        else SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex) + 1);
    }
}
