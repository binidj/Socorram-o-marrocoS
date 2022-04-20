using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using Prototyping.Scripts.Controllers;

namespace Prototyping.Scripts.Entities
{
    public class EnemyMovement : MonoBehaviour
    {
        private Vector3 target;
        private float speed = 2.0f;
        private int targetIndex = -1;
        private bool canMove { get; set; } = false;
        private EnemyController enemyController;
        [SerializeField] private EnemyStats enemyStats;
        private Animator animator;

        private void Awake()
        {
            speed = enemyStats.speed;
            animator = gameObject.GetComponent<Animator>();
        }
        
        private void Update()
        {
            if (!canMove) return;
            
            if (transform.position == target)
            {
                FollowNextTarget();
            }

            Vector2 moveDirection = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
            moveDirection.Normalize();

            animator.SetFloat("Horizontal", moveDirection.x);
            animator.SetFloat("Vertical", moveDirection.y);
        }

        private void FixedUpdate() {
            if (!canMove) return;

            float maxDistance = speed * Time.fixedDeltaTime;

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
            animator.SetBool("CanMove", true);
            FollowNextTarget();
        }

        public void StopMovement()
        {
            canMove = false;
            animator.SetBool("CanMove", false);
        }
    }
}