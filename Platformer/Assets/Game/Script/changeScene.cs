using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    public string sceneToLoad = "MainMenu";

    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("change scene to " + sceneToLoad);
            changeTheScene();
        }
    }

    private void changeTheScene(){
            SceneManager.LoadScene(sceneToLoad);
    }

    void Update()
    {
        
    }
}
