using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameObject ContactPlayerLogic;
    private Animator animator;
    private GestionPv gestionPv;
    private InflictDamage inflictDamage;
    public void Awake(){
        animator = GetComponent<Animator>();
        gestionPv = ContactPlayerLogic.GetComponent<GestionPv>();
        inflictDamage = ContactPlayerLogic.GetComponent<InflictDamage>();
    }

    void Update() {
        if (gestionPv.EntityHp <= 0 && animator.GetBool("isAlive")) {
            animator.SetBool("isAlive", false);
            inflictDamage.canInflictDamage = false;
            Invoke("DestroyGameObject", 3f);
        }
        else if (gestionPv.GetIsAlive() && inflictDamage.isAttacking) {
            animator.SetBool("isAttacking", true);
        }
        else if (gestionPv.GetIsAlive() && ! inflictDamage.isAttacking) {
            animator.SetBool("isAttacking", false);
        }
    }
   
    private void DestroyGameObject() {
        Destroy(gameObject);
    }
}
