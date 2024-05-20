using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 startPos;
    public List<string> tagsCibles = new List<string>(); // Utiliser une liste pour stocker plusieurs tags cibles
    public int damage = 1;
    public int swordDamage = 10;
    public GameObject swordObject;

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

public void Attack()
{
    swordObject.GetComponent<Sword>().Attack();
    /*
    if (swordObject != null)
    {
        // Utiliser OverlapBox pour détecter une collision partielle avec tous les objets à la fois
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(swordObject.transform.position, swordObject.GetComponent<BoxCollider2D>().size, 0f);

        // Parcourir tous les colliders en collision
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Entity"))
            {
                Debug.Log("damage sword");
                // Vérifier si l'objet a un script de santé
                GestionPv gestionPv = hitCollider.GetComponent<GestionPv>();
                if (gestionPv != null)
                {
                    // Infliger des dégâts à l'objet
                    gestionPv.TakeDamage(swordDamage);
                }
            }
        }
    }
    else
    {
        Debug.LogWarning("Sword object not assigned!");
    }
    */
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
