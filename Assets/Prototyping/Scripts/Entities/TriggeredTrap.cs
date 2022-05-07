using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Modfiers;

namespace Prototyping.Scripts.Entities
{
    public class TriggeredTrap : MonoBehaviour, ITrap
    {
        private bool firstClick = true;
        public bool isPlacing {get; set;} = false;
        [field: SerializeField] public TrapType trapType { get; private set; }
        [field: SerializeField] public float value { get; private set; }
        [field: SerializeField] public float lifeSpan { get; private set; }
        private Animator[] animators;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animators = transform.parent.GetComponentsInChildren<Animator>();    
        }

        public void Trigger()
        {
            if (firstClick) 
            {
                firstClick = false;
                return;
            }
            audioSource.Play();
            isPlacing = true;
            
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(gameObject.GetComponent<Collider2D>(), filter, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.tag != "WaveEnemies") continue;
                if (trapType == TrapType.Health)
                {
                    collider.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(value);
                }
                else if (trapType == TrapType.Speed)
                {
                    collider.gameObject.GetComponent<EnemyMovement>().AddLifeSpanTrap(new BaseModfier(value, lifeSpan));
                }
            }

            GameObject trap = gameObject.transform.parent.gameObject;
            StartCoroutine(DestroyTrap(trap));
        }

        IEnumerator DestroyTrap(GameObject trap)
        {
            trap.tag = "Respawn";
            foreach (var animator in animators)
            {
                animator.SetBool("triggerTrap", true);
            }
            yield return new WaitForSeconds(1f);
            trap.SetActive(false);
        }

        public IEnumerator EnableTrap()
        {
            yield return new WaitForSeconds(0.1f);
            firstClick = false;
        }
    }
}
