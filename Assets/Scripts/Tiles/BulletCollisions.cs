using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisions : MonoBehaviour
{
    public bool timerOn = false;
    public bool timerUsed = false;
    public int numberOfSeconds = 3; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator timer(){
        yield return new WaitForSeconds(numberOfSeconds);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(!timerOn && !timerUsed){
            StartCoroutine(timer());
            timerUsed = true;
            timerOn = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Destroy")){
            Destroy(this.gameObject);
        }
    }
}
