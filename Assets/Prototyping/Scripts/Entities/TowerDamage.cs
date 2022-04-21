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

        private void Start()
        {
            damagePerSecond = towerStats.damagePerSecond;
        }

        private void Update()
        {
            foreach (GameObject gameObject in activeEnemies)
            {
                gameObject.GetComponent<EnemyHealthManager>()?.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (activeEnemies.Contains(other.gameObject)) return;
            activeEnemies.Add(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other) {
            activeEnemies.Remove(other.gameObject);
        }

    }
}