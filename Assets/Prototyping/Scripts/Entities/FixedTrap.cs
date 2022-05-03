using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.Entities
{
    public class FixedTrap : MonoBehaviour, ITrap
    {
        [field: SerializeField] public TrapType trapType { get; private set; }
        [field: SerializeField] public float value { get; private set; }
        public bool isPlacing {get; set;} = false;
        public void Trigger() {}
    }
}

