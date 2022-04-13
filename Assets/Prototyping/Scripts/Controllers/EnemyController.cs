using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    private List<GameObject> enemies = new List<GameObject>();
    private List<Vector3> pathPositions;
    private Vector3 walkRightOffset = new Vector3(100000f, 0, 0);
    [SerializeField] private float destroyDelay = 0.2f;

    private void OnEnable() {
        EnemySpawner.spawnEnemyEvent += ReceiveEnemy;
        StartWave.startWaveEvent += GetPathPositions;
    }

    private void OnDisable()
    {
        EnemySpawner.spawnEnemyEvent -= ReceiveEnemy;
        StartWave.startWaveEvent -= GetPathPositions;
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
        StartCoroutine(DestroyEnemy(other.gameObject));
    }
    
    private IEnumerator DestroyEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(destroyDelay);
        enemy.SetActive(false);    
        DealDamageToPlayer();
    }

    private void DealDamageToPlayer()
    {
        // Update UI
        Debug.Log("Player took damage");
    }
}
