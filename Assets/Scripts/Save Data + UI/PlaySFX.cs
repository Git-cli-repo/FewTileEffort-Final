using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Method to play the sound effect with a given AudioClip
    public void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            Debug.Log("Clip: " + clip.name);
            audioSource.Play();
            Debug.Log("Played Clip " + clip.name);
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing.");
        }
    }
}
