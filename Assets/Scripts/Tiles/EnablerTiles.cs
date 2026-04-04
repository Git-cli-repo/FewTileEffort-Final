using UnityEngine;
public class EnablerTiles: MonoBehaviour {
    public string objectTag;
    public bool enabledMethod;
    public GameObject[] objectsWithTag;
    void Start(){
        objectsWithTag = GameObject.FindGameObjectsWithTag(objectTag);
    }
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("PlayerObj")){
            if(enabledMethod == true){
                foreach(GameObject Gob in objectsWithTag){
                    Gob.SetActive(true);
                }

            } else if (enabledMethod == false){
                foreach (GameObject Gob in objectsWithTag)
                {
                    Gob.SetActive(false);
                }
            }
        }
    }
}