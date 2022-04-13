using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    private float health;
    [SerializeField] private EnemyStats enemyStats;

    private void Start()
    {
        health = enemyStats.health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        // TODO : die behavior
        gameObject.SetActive(false);
    }
}
