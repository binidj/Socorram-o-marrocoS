using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Entities;

namespace Prototyping.Scripts.Controllers
{
    public class TrapMonitor : MonoBehaviour
    {
        private bool waveStarted = false;
        private void OnEnable() 
        {
            StartWave.startWaveEvent += OnWaveStart;
        }

        private void OnDisable()
        {
            StartWave.startWaveEvent -= OnWaveStart;
        }

        private void OnWaveStart()
        {
            waveStarted = true;
        }

        private void Update()
        {
            if (waveStarted)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 15f, LayerMask.GetMask("Traps"));

                    if (hit.collider != null)
                    {
                        ITrap trap = hit.collider.gameObject.GetComponentInChildren<ITrap>();
                        if (!trap.isPlacing)
                        {
                            trap.Trigger();
                            hit.collider.gameObject.SetActive(false);
                            hit.collider.gameObject.tag = "Respawn";
                        }
                        
                    }
                }
            }
        }
    }
}

