using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gérer la collision avec un objet "Player"
            Debug.Log("DEAM");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
