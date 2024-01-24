using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objetPrefab;
    public float spawnInterval = 5f;
    public float limitSpawn = 20;
    public float activationDistance = 20f;
    private int spawnNumber = 0;
    private bool isSpawning = false;

    void Start() {
        check();
    }


    void check (){
        InvokeRepeating("CheckAndStartSpawning", 0f, spawnInterval);
    }

    void CheckAndStartSpawning() {
        if (spawnNumber < limitSpawn){
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!isSpawning && Vector3.Distance(transform.position, player.transform.position) < activationDistance) {
                SpawnMonster();
            }
        }
    }

    void SpawnMonster() {
        spawnNumber++;
        Instantiate(objetPrefab, transform.position, Quaternion.identity);
    }

    // Dessine le gizmo dans l'éditeur Unity.
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
