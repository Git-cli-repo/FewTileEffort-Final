using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DarkLightTiles : MonoBehaviour
{
    [SerializeField] public GameObject[] DarkTiles;
    [SerializeField] public GameObject[] LightTiles;
    [SerializeField] public string currentActive;
    public bool isSet = false;

    [SerializeField] private bool timerOff = true;
    // Start is called before the first frame update
    void Start()
    {
        if(!isSet){
            currentActive = "light";
            DarkTiles = GameObject.FindGameObjectsWithTag("Dark");
            LightTiles = GameObject.FindGameObjectsWithTag("Light");
            foreach (GameObject gob in DarkTiles){
                gob.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    IEnumerator waitTime(){
        timerOff = false;
        yield return new WaitForSeconds(1);
        timerOff = true;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(timerOff == true){
            Debug.Log("You collided with trigger: " + collision.gameObject.tag);
            if(collision.gameObject.CompareTag("DL Switch")){
                Debug.Log("Congragulations! My code works a little :)");
                switch(currentActive){
                    case "dark":
                        Debug.Log("DARK");
                        foreach(GameObject gob in LightTiles){
                            gob.SetActive(true);
                            Debug.Log("LIGHT ACTIVE >> DARK");
                        }

                        foreach(GameObject gob in DarkTiles){
                            gob.SetActive(false);
                            Debug.Log("DARK INACTIVE >> DARK");
                        }

                        currentActive = "light";
                        Debug.Log(">>> LIGHT");
                        collision.gameObject.SetActive(false);
                        StartCoroutine(waitTime());
                        break;
                        

                    case "light":
                        Debug.Log("LIGHT");    
                        foreach(GameObject gob in LightTiles){
                            gob.SetActive(false);
                            Debug.Log("LIGHT INACTIVE >> LIGHT");
                        }

                        foreach(GameObject gob in DarkTiles){
                            gob.SetActive(true);
                            Debug.Log("DARK ACTIVE >> LIGHT");
                        }

                        currentActive = "dark";
                        Debug.Log(">>> DARK");
                        collision.gameObject.SetActive(false);
                        StartCoroutine(waitTime());
                        break;
                }
            }
        }
    }
}
