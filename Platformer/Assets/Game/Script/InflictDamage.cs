using UnityEngine;
using System.Collections.Generic; // N'oubliez pas d'ajouter ce using pour utiliser List<T>

public class InflictDamage : MonoBehaviour
{
    public List<string> tagsCibles = new List<string>(); // Utiliser une liste pour stocker plusieurs tags cibles
    public int damage = 1;
    public GameObject damageZone; // Ajoutez l'objet cible dans l'inspecteur Unity
    public bool isAttacking = false;

    public bool canInflictDamage = true;    // if we are not dead
    public float attackInterval = 1f; // Intervalle entre les attaques en secondes
    private Coroutine damageRoutine; // Stocker la référence à la coroutine pour pouvoir l'arrêter

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canInflictDamage && tagsCibles.Contains(other.tag))
        {
            damageRoutine = StartCoroutine(InflictDamageRoutine(other.gameObject));
            isAttacking = true; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canInflictDamage && tagsCibles.Contains(collision.gameObject.tag))
        {
            damageRoutine = StartCoroutine(InflictDamageRoutine(collision.gameObject));
            isAttacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (tagsCibles.Contains(other.tag))
        {
            isAttacking = false;
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine); // Arrêter la coroutine si elle est en cours d'exécution
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (tagsCibles.Contains(collision.gameObject.tag))
        {
            isAttacking = false;
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine); // Arrêter la coroutine si elle est en cours d'exécution
            }
        }
    }

    private System.Collections.IEnumerator InflictDamageRoutine(GameObject target)
    {
        while (true)
        {
            // Attaquer la cible
            GestionPv gestionPv = target.GetComponent<GestionPv>();

            // Vérifier si l'objet a un script GestionPointsDeVie
            if (gestionPv != null)
            {
                gestionPv.TakeDamage(damage);
            }

            // Attendre avant la prochaine attaque
            yield return new WaitForSeconds(attackInterval);
        }
    }
}
