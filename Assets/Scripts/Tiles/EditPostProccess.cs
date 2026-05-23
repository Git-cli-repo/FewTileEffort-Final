using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class EditPostProccessing: MonoBehaviour {
    public PostProcessProfile postProcessProfile;
    public PostProcessVolume postProcessVolume;
    void Start(){

    }
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("PlayerObj")){
            postProcessVolume.profile = postProcessProfile;
        }
    }
}