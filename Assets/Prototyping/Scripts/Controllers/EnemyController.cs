using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    private List<GameObject> enemies = new List<GameObject>();
    private List<Vector3> pathPositions;

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
            // TODO: deal with finishing path
            return new Vector3(100000000f, 0f, 0f);
        }
        return pathPositions[index];
    }
}
