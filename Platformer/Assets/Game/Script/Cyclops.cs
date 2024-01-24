using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cyclops : MonoBehaviour
{
    public GameObject ContactPlayerLogic;
    private Animator animator;
    private GestionPv gestionPv;
    private InflictDamage inflictDamage;
    private Vector3 previousPosition; // Ajout d'une variable pour stocker la position précédente

    public void Awake(){
        animator = GetComponent<Animator>();
        gestionPv = ContactPlayerLogic.GetComponent<GestionPv>();
        inflictDamage = ContactPlayerLogic.GetComponent<InflictDamage>();

        previousPosition = transform.position;
    }

    void Update() {
        if (gestionPv.EntityHp <= 0 && animator.GetBool("isAlive")) {
            animator.SetBool("isAlive", false);
            inflictDamage.canInflictDamage = false;
            Invoke("DestroyGameObject", 3f);
        }
        else if (gestionPv.GetIsAlive() && inflictDamage.isAttacking) {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
        }
        else if (gestionPv.GetIsAlive() && ! inflictDamage.isAttacking) {
            animator.SetBool("isAttacking", false);
        }
        if (gestionPv.GetIsAlive() && ! inflictDamage.isAttacking){
            CheckMovement();
        }

    }

    public void animationThrow(){
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isThrowing", true);
    }

    public void animationThrowCancel(){
        animator.SetBool("isThrowing", false);
    }
   
    private void CheckMovement()
    {
        // Comparer la position actuelle avec la position précédente en X
        float movementX = transform.position.x - previousPosition.x;

        // Mettre à jour le paramètre isMoving en fonction du mouvement en X
        animator.SetBool("isRunning", Mathf.Abs(movementX) > 0.01f);

        // Mettre à jour la position précédente
        previousPosition = transform.position;
    }

    private void DestroyGameObject() {
        Destroy(gameObject);
    }
}
