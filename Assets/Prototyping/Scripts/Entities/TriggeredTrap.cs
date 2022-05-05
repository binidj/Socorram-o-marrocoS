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

        // private void OnEnable() 
        // {
        //     StartWave.startWaveEvent += EnableTriggerTrap;
        // }

        // private void OnDisable() 
        // {
        //     StartWave.startWaveEvent -= EnableTriggerTrap;
        // }

        // private void EnableTriggerTrap()
        // {
        //     canTriggerTrap = true;
        // }

        // private void OnMouseUp() 
        // {
        //     if (isPlacing) return;
        //     // if (!canTriggerTrap) return;
        //     // canTriggerTrap = false;
            
        //     if (Input.GetMouseButtonUp(0))
        //     {
        //         Vector3 mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        //         RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 15f, LayerMask.GetMask("Traps"));

        //         if (hit.collider != null)
        //         {

        //         }
        //         Trigger();

        //         gameObject.transform.parent.gameObject.SetActive(false);
        //         gameObject.transform.parent.gameObject.tag = "Respawn";
        //     }
            
        //     // Destroy(gameObject.transform.parent.gameObject);
        // }

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
            
            // gameObject.transform.parent.gameObject.SetActive(false);
            // gameObject.transform.parent.gameObject.tag = "Respawn";
        }

        IEnumerator DestroyTrap(GameObject trap)
        {
            foreach (var animator in animators)
            {
                animator.SetBool("triggerTrap", true);
            }
            yield return new WaitForSeconds(1.5f);
            trap.SetActive(false);
            trap.tag = "Respawn";
        }
    }
}
