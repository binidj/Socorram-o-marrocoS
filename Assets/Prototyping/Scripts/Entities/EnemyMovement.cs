using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using Prototyping.Scripts.Controllers;
using Prototyping.Scripts.Modfiers;

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
        private List<BaseModfier> speedModifiers = new List<BaseModfier>();
        private float areaModifier = 1f;
        private List<GameObject> traps = new List<GameObject>();

        private void Awake()
        {
            speed = enemyStats.speed;
            animator = gameObject.GetComponent<Animator>();
        }
        
        private void OnTriggerEnter2D(Collider2D other) 
        {    
            if (other.gameObject.GetComponent<FixedTrap>()?.trapType == TrapType.Speed)
            {
                traps.Add(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            if (other.gameObject.GetComponent<FixedTrap>()?.trapType == TrapType.Speed)
            {
                traps.Remove(other.gameObject);
            }
        }

        private void Update()
        {
            UpdateModifiersLifeSpan();
            areaModifier = 1f;
            UpdateAreaModifier();
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

        private void UpdateAreaModifier()
        {
            foreach (GameObject trap in traps)
            {
                areaModifier *= trap.GetComponent<FixedTrap>().value;
            }
        }

        private void UpdateModifiersLifeSpan()
        {
            List<int> modfiersToRemove = new List<int>();

            for (int i = 0; i < speedModifiers.Count; i++)
            {
                var modfier = speedModifiers[i];
                modfier.lifeSpan -= Time.deltaTime;
                if (modfier.lifeSpan <= 0f)
                {
                    modfiersToRemove.Add(i);
                }
            }

            speedModifiers.Reverse();

            foreach (int index in modfiersToRemove)
            {
                speedModifiers.RemoveAt(index);
            }
        }

        private float GetSpeedModfier()
        {
            float speedModfier = 1.0f;

            foreach (var modfier in speedModifiers)
            {
                speedModfier *= modfier.value;
            }

            return speedModfier;
        }

        public void AddLifeSpanTrap(BaseModfier speedModifier)
        {
            speedModifiers.Add(speedModifier);
        }

        private void FixedUpdate() {
            if (!canMove) return;

            float speedModfier = GetSpeedModfier();

            float maxDistance = speed * speedModfier * areaModifier * Time.fixedDeltaTime;

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

        public void AddModfier(BaseModfier modfier)
        {
            speedModifiers.Add(modfier);
        }
    }
}