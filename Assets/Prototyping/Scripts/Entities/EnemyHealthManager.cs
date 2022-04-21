using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Entities
{
    public class EnemyHealthManager : MonoBehaviour
    {
        private float health;
        [SerializeField] private EnemyStats enemyStats;
        private Animator animator;
        [SerializeField] private float deathAnimationTime = 2.0f;
        private bool dead = false;
        private EnemyMovement enemyMovement;

        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            enemyMovement = gameObject.GetComponent<EnemyMovement>();
            health = enemyStats.health;
            animator.SetFloat("Health", health);
        }

        public void TakeDamage(float damage)
        {
            if (dead) return;
            health -= damage;
            animator.SetFloat("Health", health);
            if (health <= 0)
            {
                dead = true;
                enemyMovement.StopMovement();
                StartCoroutine(Die());
            }
        }

        private void OnTriggerStay2D(Collider2D other) 
        {
            FixedTrap fixedTrap = other.gameObject.GetComponent<FixedTrap>();
            if (fixedTrap != null && fixedTrap.trapType == TrapType.Health)
            {
                TakeDamage(fixedTrap.value * Time.deltaTime);
            }
        }

        private IEnumerator Die()
        {
            yield return new WaitForSeconds(deathAnimationTime);
            gameObject.SetActive(false);
        }
    }
}