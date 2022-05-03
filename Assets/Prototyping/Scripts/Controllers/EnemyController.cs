using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Entities;
using UnityEngine.UI;

namespace Prototyping.Scripts.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private GridController gridController;
        private List<GameObject> enemies = new List<GameObject>();
        private List<Vector3> pathPositions;
        private Vector3 walkRightOffset = new Vector3(100000f, 0, 0);
        [SerializeField] private int playerHealth = 3;
        [SerializeField] private Text textHealth;
        [SerializeField] private LevelController levelController;
        private int deathCount = 0;

        public void UpdateDeathCount()
        {
            deathCount += 1;
            if (deathCount == EnemySpawner.enemyAmount)
            {
                levelController.GoToNextLevel();
            }
        }

        private void OnEnable() {
            EnemySpawner.spawnEnemyEvent += ReceiveEnemy;
            StartWave.startWaveEvent += GetPathPositions;
            EnemyHealthManager.updateDeathCountEvent += UpdateDeathCount;
        }

        private void OnDisable()
        {
            EnemySpawner.spawnEnemyEvent -= ReceiveEnemy;
            StartWave.startWaveEvent -= GetPathPositions;
            EnemyHealthManager.updateDeathCountEvent -= UpdateDeathCount;
        }

        private void Start()
        {
            textHealth.text = $"Health: {playerHealth}";
        }

        private void ReceiveEnemy(GameObject newEnemy)
        {
            enemies.Add(newEnemy);
            EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
            enemyMovement.SetController(this);
        }

        private void GetPathPositions()
        {
            pathPositions = gridController.GetPathPositions();
        }

        public Vector3 GetPositionFromIndex(int index)
        {
            if (index >= pathPositions.Count)
            {
                Vector3 lastPosition = (pathPositions.Count != 0) ? pathPositions[pathPositions.Count - 1] : new Vector3();
                return lastPosition + walkRightOffset;
            }
            return pathPositions[index];
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag != "WaveEnemies") return;
            DestroyEnemy(other.gameObject);
        }
        
        private void DestroyEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
            if (!DealDamageToPlayer())
            {
                UpdateDeathCount();
            }
        }

        private bool DealDamageToPlayer()
        {
            playerHealth -= 1;
            if (playerHealth == 0) 
            {
                levelController.ReloadLevel();
                return true;
            }
            else if (playerHealth < 0)
            {
                return true;
            }
            textHealth.text = $"Health: {playerHealth}";
            return false;
        }
    }
}