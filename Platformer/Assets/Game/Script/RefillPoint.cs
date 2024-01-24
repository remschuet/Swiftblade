using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class RefillPoint : MonoBehaviour
{
    public int amountMana = 100;
    public int amountHp = 100;
    public GameObject panneau;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().refillMana(amountMana);
            other.gameObject.GetComponent<GestionPv>().Heal(amountHp);
            Destroy(panneau);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
