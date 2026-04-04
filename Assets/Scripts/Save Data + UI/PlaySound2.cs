using UnityEngine;

public class SoundEffectPlayer2 : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.Play();
        }
    
    }
}