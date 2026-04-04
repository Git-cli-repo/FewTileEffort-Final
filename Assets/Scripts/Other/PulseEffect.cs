using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public Color targetColor = Color.white; // The color to pulse to (white)
    public float pulseDuration = 1.0f; // Duration for one pulse cycle
    private Color originalColor; // The original color of the object
    private SpriteRenderer spriteRenderer; // test
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("PulseEffect script requires a SpriteRenderer component.");
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Calculate the lerp value
        float lerpValue = Mathf.PingPong(Time.time / pulseDuration, 1.0f);
        
        // Lerp between the original color and the target color
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, lerpValue);
    }
}
