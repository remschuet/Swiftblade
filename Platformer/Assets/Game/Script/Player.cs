using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 startPos;
    public List<string> tagsCibles = new List<string>(); // Utiliser une liste pour stocker plusieurs tags cibles
    public int damage = 1;
    private GestionPv gestionPv;
    private Animator animator;
    private bool reviving = false;

    public void Awake(){
        animator = GetComponent<Animator>();
        gestionPv = GetComponent<GestionPv>();
    }
    private void Start(){
        startPos = transform.position;
    }
    
    private void newSavePoint(){
        Debug.Log("new");
        startPos = transform.position;
    }

   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tagsCibles.Contains(collision.gameObject.tag))
        {
            GestionPv gestionPv = collision.GetComponent<GestionPv>();
            if (gestionPv != null) {
                if (animator.GetBool("isDashingAttacking")) {
                    gestionPv.TakeDamage(damage);
                }
            }
        }        
        else if (collision.CompareTag("SavePoint")){
            newSavePoint();
        }
    }

    void Update() {
        if (! gestionPv.GetIsAlive() && ! reviving){
            Die();
        }
    }

    void Die() {
        reviving = true;
        StartCoroutine(Respawn(2f));
    }

    IEnumerator Respawn(float duration){
        yield return new WaitForSeconds(duration);
        transform.position = startPos;
        gestionPv.Respauwn();
        animator.SetBool("isAlive", true);
        reviving = false;
    }
}
