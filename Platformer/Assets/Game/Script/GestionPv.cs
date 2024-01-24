using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionPv : MonoBehaviour
{
    public int EntityHp = 100;
    public bool canTakeDamage = true;
    public Image healthBar;
    private bool isAlive = true;
    private int EntityHpDefault;

    public void Start(){
        EntityHpDefault = EntityHp;
    }

    public bool GetIsAlive(){
        return isAlive;
    }
    public void TakeDamage(int damage) {
        if (canTakeDamage) {
            if (healthBar){
                healthBar.fillAmount = EntityHp / 100f;
            }
            EntityHp -= damage;
        }
        if (EntityHp <= 0){
            isAlive = false;
        }
    }

    public void Heal(int healing){
        EntityHp += healing;
        if (EntityHp > EntityHpDefault){
            EntityHp = EntityHpDefault;
        }
        healthBar.fillAmount = EntityHp / 100f;
    }

    private void DestroyGameObject() {
        Destroy(gameObject);
    }

    public void Respauwn(){
        Debug.Log("respauwn");
        isAlive = true;
        EntityHp = EntityHpDefault;
        if (healthBar){
            healthBar.fillAmount = EntityHp / 100f;
        }        
    }
}
