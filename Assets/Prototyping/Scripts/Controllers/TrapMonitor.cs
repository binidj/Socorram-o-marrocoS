using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Entities;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Controllers
{
    public class TrapMonitor : MonoBehaviour
    {
        private bool waveStarted = false;
        [SerializeField] TrapsController trapsController;
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
            if (waveStarted && !trapsController.isPlacingTrap)
            {
                
                Vector3 mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 15f, LayerMask.GetMask("PlacedTrap"));
                
                if (hit.collider != null && CursorController.Instance.cursorType != CursorType.CanTriggerTrap)
                    CursorController.Instance.SetActiveCursorType(CursorType.CanTriggerTrap);

                if (hit.collider == null && CursorController.Instance.cursorType != CursorType.Default)
                    CursorController.Instance.SetActiveCursorType(CursorType.Default);

                if (Input.GetMouseButtonUp(0))
                {
                    if (hit.collider != null)
                    {
                        ITrap trap = hit.collider.gameObject.GetComponentInChildren<ITrap>();
                        if (!trap.isPlacing)
                        {
                            trap.Trigger();
                            CursorController.Instance.SetActiveCursorType(CursorType.Default);
                        }
                    }
                }
            }
        }
    }
}

