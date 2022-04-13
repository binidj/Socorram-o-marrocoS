using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 target;
    private float speed = 2.0f;
    private int targetIndex = -1;
    private bool canMove { get; set; } = false;
    private EnemyController enemyController;
    [SerializeField] private EnemyStats enemyStats;

    private void Start()
    {
        speed = enemyStats.speed;
    }
    
    private void Update()
    {
        if (!canMove) return;

        if (transform.position == target)
        {
            FollowNextTarget();
        }
        
        float maxDistance = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target, maxDistance);
    }

    private void FollowNextTarget()
    {
        targetIndex += 1;
        target = enemyController.GetPositionFromIndex(targetIndex);
    }

    public void SetController(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        canMove = true;
        FollowNextTarget();
    }
}
