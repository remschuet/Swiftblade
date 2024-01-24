using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject door;
    public float timeDetroyDoor = 0f;
    private GestionPv gestionPv;
    
    void Awake() {
        gestionPv = GetComponent<GestionPv>();
    }

    void Update() {
        if (! gestionPv.GetIsAlive()){
            Debug.Log("destroy");
            Invoke("DestroyDoor", timeDetroyDoor);
        }
    }

    void DestroyDoor() {
        Destroy(gameObject);
        if (door != null) {
            Destroy(door);
        }
    }
}
