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

        private void OnEnable() {
            EnemySpawner.spawnEnemyEvent += ReceiveEnemy;
            StartWave.startWaveEvent += GetPathPositions;
        }

        private void OnDisable()
        {
            EnemySpawner.spawnEnemyEvent -= ReceiveEnemy;
            StartWave.startWaveEvent -= GetPathPositions;
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
            DestroyEnemy(other.gameObject);
        }
        
        private void DestroyEnemy(GameObject enemy)
        {
            enemy.SetActive(false);    
            DealDamageToPlayer();
        }

        private void DealDamageToPlayer()
        {
            playerHealth -= 1;
            if (playerHealth <= 0) levelController.ReloadLevel();
            textHealth.text = $"Health: {playerHealth}";
        }
    }
}