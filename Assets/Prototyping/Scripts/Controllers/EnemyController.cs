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
        [SerializeField] private GameObject victoryUI;
        [SerializeField] private GameObject defeatUI;
        [SerializeField] private AudioClip victorySound;
        [SerializeField] private AudioClip defeatSound;
        [SerializeField] private GameObject placeableUI;
        [SerializeField] private AudioSource background;
        private AudioSource audioSource;
        private bool levelEnded = false;

        public void UpdateDeathCount()
        {
            deathCount += 1;
            if (deathCount == EnemySpawner.enemyAmount)
            {
                if (!defeatUI.activeSelf) SetEndLevelUI(victoryUI, victorySound);
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
            audioSource = GetComponent<AudioSource>();
        }

        private void ReceiveEnemy(GameObject newEnemy)
        {
            enemies.Add(newEnemy);
            EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
            enemyMovement.SetController(this);
            if (levelEnded) newEnemy.SetActive(false);
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
                if (!defeatUI.activeSelf) SetEndLevelUI(defeatUI, defeatSound);
                textHealth.text = $"Health: {playerHealth}";
                return true;
            }
            else if (playerHealth < 0)
            {
                return true;
            }
            textHealth.text = $"Health: {playerHealth}";
            return false;
        }

        private void SetEndLevelUI(GameObject nextUI, AudioClip audioClip)
        {
            StopEnemies();
            levelEnded = true;
            placeableUI.SetActive(false);
            background.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
            nextUI.SetActive(true);
        }

        private void StopEnemies()
        {
            foreach (var enemy in enemies)
            {
                enemy.SetActive(false);
            }
        }
    }
}