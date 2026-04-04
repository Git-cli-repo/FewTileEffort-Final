using UnityEngine;

public class SoundEffectPlayer3 : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            audioSource.Play();
        }
    
    }
}