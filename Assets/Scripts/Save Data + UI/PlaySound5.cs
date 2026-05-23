using UnityEngine;

public class SoundEffectPlayer5 : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Button")){
            audioSource.Play();
        }
    }
}