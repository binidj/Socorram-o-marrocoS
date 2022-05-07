using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using Prototyping.Scripts.Entities;

namespace Prototyping.Scripts.Controllers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyWave currentWave;
        private bool isSpawning = false;
        public static int enemyAmount = -1;
        private int enemyIndex = -1;
        private float timeToWait = 0f;
        public delegate void SpawnEnemy(GameObject enemy);
        public static event SpawnEnemy spawnEnemyEvent;
        private List<GameObject> enemies;
        [SerializeField] private GameObject baseEnemy;

        private void OnEnable() {
            StartWave.startWaveEvent += BeginWave;
        }

        private void OnDisable() {
            StartWave.startWaveEvent -= BeginWave;
        }

        private void Awake()
        {
            baseEnemy.SetActive(false);
            enemies = new List<GameObject>();
            foreach (var enemyData in currentWave.enemies)
            {
                enemies.Add(Instantiate(enemyData.enemy, transform.position, Quaternion.identity, transform));
            }
        }

        private void Start() 
        {
            enemyAmount = currentWave.enemies.Count;
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
            GameObject newEnemy = enemies[enemyIndex]; //Instantiate(currentWave.enemies[enemyIndex].enemy, gameObject.transform.position, Quaternion.identity);
            newEnemy.SetActive(true);
            bool isLastEnemy = (enemyIndex + 1) == currentWave.enemies.Count;
            if (isLastEnemy) newEnemy.GetComponent<EnemyHealthManager>().isLastEnemy = true;
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