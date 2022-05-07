using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Entities;
using UnityEngine.UI;
using System.Linq;

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
        [SerializeField] private int maxPoints = 100;
        private List<GameObject> points = new List<GameObject>();
        private List<SpriteRenderer> pointsSprites = new List<SpriteRenderer>();
        [SerializeField] GameObject pointTemplate;
        private int pathSize = 0;
        private Color32 colorRed = new Color32(255, 0, 0, 255);
        private Color32 colorGreen = new Color32(0, 255, 0, 255);

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
            InitPoints();
        }

        private void InitPoints()
        {
            pointTemplate.SetActive(false);
            for (int i = 0; i < maxPoints; i++)
            {
                points.Add(Instantiate(pointTemplate, new Vector3(-1000,1000,0), Quaternion.identity, transform));
                pointsSprites.Add(points.Last().GetComponent<SpriteRenderer>());
            }
                
        }

        private void AlignPoitsToPath()
        {
            for (int i = 0; i < pathSize; i++)
                points[i].transform.position = pathPositions[i];
        }

        private bool HasTrapOnPoint(GameObject point)
        {
            RaycastHit2D hit = Physics2D.Raycast(point.transform.position, Vector2.zero, 15f, LayerMask.GetMask("Traps"));
            return hit.collider != null && hit.collider.transform.parent.tag != "Respawn";
        }

        private void SetPointsColors()
        {
            for (int i = 0; i < pathSize; i++)
                if (HasTrapOnPoint(points[i]))
                    pointsSprites[i].color = colorRed;
                else
                    pointsSprites[i].color = colorGreen;
        }

        public void SetActivePoints(bool active)
        {
            if (active)
                SetPointsColors();
            
            for (int i = 0; i < pathSize; i++)
                points[i].SetActive(active);
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
            pathSize = pathPositions.Count;
            AlignPoitsToPath();
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