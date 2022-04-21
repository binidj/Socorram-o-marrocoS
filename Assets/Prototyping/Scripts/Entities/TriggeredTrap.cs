using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Modfiers;

namespace Prototyping.Scripts.Entities
{
    public class TriggeredTrap : MonoBehaviour
    {
        private bool canTriggerTrap = false;
        [field: SerializeField] public TrapType trapType { get; private set; }
        [field: SerializeField] public float value { get; private set; }
        [field: SerializeField] public float lifeSpan { get; private set; }

        private void OnEnable() 
        {
            StartWave.startWaveEvent += EnableTriggerTrap;
        }

        private void OnDisable() 
        {
            StartWave.startWaveEvent -= EnableTriggerTrap;
        }

        private void EnableTriggerTrap()
        {
            canTriggerTrap = true;
        }

        private void OnMouseUp() 
        {
            if (!canTriggerTrap) return;
            canTriggerTrap = false;
            
            Trigger();

            gameObject.transform.parent.gameObject.SetActive(false);
        }

        private void Trigger()
        {
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
        }
    }
}
