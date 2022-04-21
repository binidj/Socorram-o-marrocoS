using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Controllers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyWave currentWave;
        private bool isSpawning = false;
        private int enemyIndex = -1;
        private float timeToWait = 0f;
        public delegate void SpawnEnemy(GameObject enemy);
        public static event SpawnEnemy spawnEnemyEvent;

        private void OnEnable() {
            StartWave.startWaveEvent += BeginWave;
        }

        private void OnDisable() {
            StartWave.startWaveEvent -= BeginWave;
        }

        private void BeginWave()
        {
            isSpawning = true;
            enemyIndex = -1;
            SelectNextEnemy();   
        }

        private void Update()
        {
            if (!isSpawning) return;
            
            if (timeToWait > 0f)
            {
                timeToWait -= Time.deltaTime;
                if (timeToWait <= 0)
                {
                    Spawn();
                    SelectNextEnemy();
                }
            }
        }

        private void Spawn()
        {
            GameObject newEnemy = Instantiate(currentWave.enemies[enemyIndex].enemy, gameObject.transform.position, Quaternion.identity);
            spawnEnemyEvent?.Invoke(newEnemy);
        }

        private void SelectNextEnemy()
        {
            enemyIndex += 1;
            if (enemyIndex == currentWave.enemies.Count)
            {
                isSpawning = false;
                return;
            }
            timeToWait = currentWave.enemies[enemyIndex].timeToWait;
        }
    }
}