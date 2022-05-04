using System.Collections;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using UnityEngine.UI;

namespace Prototyping.Scripts.Entities
{
    public class EnemyHealthManager : MonoBehaviour
    {
        private float health;
        [SerializeField] private EnemyStats enemyStats;
        private Animator animator;
        [SerializeField] private float deathAnimationTime = 2.0f;
        private bool dead = false;
        private EnemyMovement enemyMovement;
        [SerializeField] private Slider _slider;
        public bool isLastEnemy {get; set;} = false;
        public delegate void UpdateDeathCount();
        public static event UpdateDeathCount updateDeathCountEvent;

        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            enemyMovement = gameObject.GetComponent<EnemyMovement>();
            health = enemyStats.health;
            animator.SetFloat("Health", health);
            _slider.maxValue = health;
            _slider.value = health;
        }

        public void TakeDamage(float damage)
        {
            if (dead) return;
            health -= damage;
            _slider.value = health;
            animator.SetFloat("Health", health);
            if (health <= 0)
            {
                dead = true;
                enemyMovement.StopMovement();
                StartCoroutine(Die());
            }
        }

        private void OnTriggerStay2D(Collider2D other) 
        {
            FixedTrap fixedTrap = other.gameObject.GetComponent<FixedTrap>();
            if (fixedTrap != null && fixedTrap.trapType == TrapType.Health)
            {
                TakeDamage(fixedTrap.value * Time.deltaTime);
            }
        }

        private IEnumerator Die()
        {
            updateDeathCountEvent?.Invoke();
            yield return new WaitForSeconds(deathAnimationTime);
            gameObject.SetActive(false);
        }
    }
}