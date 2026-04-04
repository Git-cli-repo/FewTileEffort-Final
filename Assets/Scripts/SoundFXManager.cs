using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    public static SoundFXManager instance;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, Transform posTransform, float volume){
        AudioSource source = Instantiate(soundFXObject, posTransform.position, Quaternion.identity);
        source.clip = clip;
        source.volume = volume;
        source.Play();
        float clipLength = source.clip.length;
        Destroy(source.gameObject, clipLength);
    }
}
