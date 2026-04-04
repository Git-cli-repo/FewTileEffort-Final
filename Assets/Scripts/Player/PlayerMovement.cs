using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    [SerializeField] public float speed;
    [SerializeField] public float jumpHeight;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private bool isOnGripTile = false;
    public PlaySoundEffect playSound;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public Sprite[] skins; // Array of available skins
    private SpriteRenderer spriteRenderer;
    // New parameter to set the desired scale (size) for the player's sprite
    public Vector2 targetScale = new Vector2(1, 1); // Default scale (1, 1  
    // New parameter to offset the sprite from the BoxCollider in pixels
    public float pixelOffsetY = 0f; // Default to no offset
    public PlayerCollisions playerCollisions;
    public Sprite defaultSkin;


    void Start()
    {
        // Get the SpriteRenderer component attached to the player
        spriteRenderer = GetComponent<SpriteRenderer>();
        string color = AchievementManager.Instance.ReadFromFile("tracker.json", "playerColor", true);
        PlayerColorChanger colorChange = GetComponent<PlayerColorChanger>();
        colorChange.ChangePlayerColor(color);
        playerCollisions = GetComponent<PlayerCollisions>();
    }

    private void Awake(){
        body = GetComponent<Rigidbody2D>();
        
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        playSound = GetComponent<PlaySoundEffect>();
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Grip")){
            isOnGripTile = true;
        }
    }

    private void Update(){
        if(!isOnGripTile){
            body.linearVelocity = new Vector2(Input.GetAxis("Horizontal")  * speed, body.linearVelocity.y);
            if(Input.GetKey(KeyCode.Space) && isGrounded()){
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpHeight);
            }
        } else if(isOnGripTile){
            body.linearVelocity = new Vector2(body.linearVelocity.x, Input.GetAxis("Vertical"));
                if(Input.GetKey(KeyCode.Space) && isGrounded()){
                    audioSource.PlayOneShot(jumpSound);
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jumpHeight);
            }
        }
        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.nextLevelKey)){
            if(CompleteAchievementsRunManager.Instance.inPracticeMode) gameObject.GetComponent<PlayerCollisions>().NextLevel();
        }
        
        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.previousLevelKey)){
             if(CompleteAchievementsRunManager.Instance.inPracticeMode) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        /* 
            if(Input.GetKeyDown(KeyCode.E)){
                SceneManager.LoadScene("Epilogue");
            } 
        */

        if(Input.GetKeyDown(KeyCode.J)){
            spriteRenderer.sprite = defaultSkin;   
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            PlayerPrefs.SetInt("skin", 20);
        } 

        if(Input.GetKeyDown(CompleteAchievementsRunManager.Instance.reloadKey)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        float currentY = transform.position.y;   
        if (currentY > 50f) 
        {
            CompleteAchievementsRunManager.Instance.CheckBounceHeight(currentY);
        }

        if (Input.GetKeyDown(KeyCode.I) && SceneManager.GetActiveScene().name == "LevelEditor")
        {
            if(!playerCollisions.isInvincible){ 
                playerCollisions.isInvincible = true;
                CompleteAchievementsRunManager.Instance.EnqueuePopup("Activated Level Editor Invincibility");
            }
            else{
                playerCollisions.isInvincible = false;
                CompleteAchievementsRunManager.Instance.EnqueuePopup("Deactivated Level Editor Invincibility");
            }
        }

        
    }

    public bool isGrounded(){
       RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
       return raycastHit.collider != null;
    }

    private bool onWall(){
       RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
       return raycastHit.collider != null;
    }

   // Apply the skin based on the saved index in PlayerPrefs and align the sprite correctly
    public void ApplySavedSkin()
    {
        int skinIndex = PlayerPrefs.GetInt("skin", 0); // Default to 0 if no skin is set

        if (skinIndex >= 0 && skinIndex < skins.Length)
        {
            // Set the sprite to the saved skin
            spriteRenderer.sprite = skins[skinIndex];

            if(PlayerPrefs.GetInt("skin") == 20){
                spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            } else {
                spriteRenderer.transform.localScale = new Vector3(targetScale.x, targetScale.y, 1);
            }
            // Apply the correct size/scale to the sprite

            // Align the sprite with the BoxCollider to prevent clipping into the floor
            AlignSpriteWithCollider();

            Debug.Log($"Applied skin: {skinIndex} with scale: {targetScale}");
        }
        else
        {
            Debug.LogError("Skin index out of range, applying default skin.");
            spriteRenderer.sprite = skins[0]; // Apply default skin if index is invalid
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1); // Reset to default scale
        }
    }

    public void OnTriggerEnter2D (Collider2D collider){
        if(collider.gameObject.CompareTag("Secret")){
            CompleteAchievementsRunManager.Instance.UnlockSecret();
        }
    }

    // Align the sprite's position with the BoxCollider to prevent clipping into the floor
    private void AlignSpriteWithCollider()
    {
        if (spriteRenderer != null && boxCollider != null)
        {
            // Get the height of the BoxCollider and the sprite bounds
            float spriteHeight = spriteRenderer.bounds.size.y;
            float colliderHeight = boxCollider.bounds.size.y;

            // Adjust the Y position so the bottom of the sprite aligns with the bottom of the BoxCollider
            float offsetY = (colliderHeight - spriteHeight) / 2f;

            // Apply the Y position adjustment to align the sprite
            spriteRenderer.transform.localPosition = new Vector3(
                spriteRenderer.transform.localPosition.x,
                offsetY,
                spriteRenderer.transform.localPosition.z
            );
        }
        else
        {
            Debug.LogError("SpriteRenderer or BoxCollider is missing.");
        }
    }
}
