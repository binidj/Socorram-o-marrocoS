using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Entities
{
    public class TowerDamage : MonoBehaviour
    {
        private float damagePerSecond;
        [SerializeField] TowerStats towerStats;
        List<GameObject> activeEnemies = new List<GameObject>();
        private Animator animator;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            damagePerSecond = towerStats.damagePerSecond;
            animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (activeEnemies.Count > 0)
                animator.SetBool("enemiesInside", true);
            else
                animator.SetBool("enemiesInside", false);
                
            EnemyHealthManager validEnemy = null;

            foreach (var enemy in activeEnemies)
            {
                EnemyHealthManager enemyHealth = enemy.GetComponent<EnemyHealthManager>();
                if (enemy.activeSelf && !enemyHealth.dead)
                {
                    validEnemy = enemyHealth;
                    break;
                }
            }
            
            if (validEnemy != null)
            {
                if (!audioSource.isPlaying) audioSource.Play();
                validEnemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
            else
            {
                if (audioSource.isPlaying) audioSource.Stop();
            }
            // foreach (GameObject gameObject in activeEnemies)
            // {
            //     gameObject.GetComponent<EnemyHealthManager>()?.TakeDamage(damagePerSecond * Time.deltaTime);
            // }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag != "WaveEnemies") return;
            if (activeEnemies.Contains(other.gameObject)) return;
            activeEnemies.Add(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.tag != "WaveEnemies") return;
            activeEnemies.Remove(other.gameObject);
        }

    }
}