using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderManager : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Collision avec l'échelle");
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Quitter l'échelle");
            rb.gravityScale = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other ){
    if (other.gameObject.CompareTag("Ladder")){
            Debug.Log("collision echelle");
            rb.gravityScale = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ladder")) {
            rb.gravityScale = 1f;
        }
    }
}
