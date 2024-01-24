using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class woodenBox : MonoBehaviour
{
    private Animator animator;
    private GestionPv gestionPv;
    public void Awake(){
        animator = GetComponent<Animator>();
        gestionPv = GetComponent<GestionPv>();
    }

    // Update is called once per frame
    void Update() {
        if (gestionPv.EntityHp <= 0 && animator.GetBool("isAlive")) {
            animator.SetBool("isAlive", false);
        }
    }
}
