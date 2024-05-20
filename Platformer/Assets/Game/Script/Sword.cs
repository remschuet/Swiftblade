using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public List<GameObject> collidingEntities = new List<GameObject>();
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Entity") && !collidingEntities.Contains(other.gameObject))
        {
            collidingEntities.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Entity") && collidingEntities.Contains(other.gameObject))
        {
            collidingEntities.Remove(other.gameObject);
        }
    }

    public void Attack()
    {
        foreach (GameObject entity in collidingEntities)
        {
            GestionPv gestionPv = entity.GetComponent<GestionPv>();
            if (gestionPv != null)
            {
                gestionPv.TakeDamage(damageAmount);
            }
        }
    }
}
