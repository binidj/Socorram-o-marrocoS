using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyData 
{
    public GameObject enemy;
    public float timeToWait;
}

[CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy Wave")]
public class EnemyWave : ScriptableObject
{
    public List<EnemyData> enemies;
}

