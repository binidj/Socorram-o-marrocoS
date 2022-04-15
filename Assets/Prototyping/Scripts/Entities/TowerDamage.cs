using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDamage : MonoBehaviour
{
    private float damagePerSecond;
    [SerializeField] TowerStats towerStats;
    List<GameObject> activeEnemies = new List<GameObject>();
    Dictionary<GameObject, int> entryCount = new Dictionary<GameObject, int>();

    private void Start()
    {
        damagePerSecond = towerStats.damagePerSecond;
    }

    private void Update()
    {
        foreach (GameObject gameObject in activeEnemies)
        {
            gameObject.GetComponent<EnemyHealthManager>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!entryCount.ContainsKey(other.gameObject))
        {
            activeEnemies.Add(other.gameObject);
            entryCount[other.gameObject] = 1;
        }
        else 
            entryCount[other.gameObject] += 1;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (entryCount.ContainsKey(other.gameObject) && entryCount[other.gameObject] > 1)
        {
            entryCount.Remove(other.gameObject);
            activeEnemies.Remove(other.gameObject);
        }
    }

}
