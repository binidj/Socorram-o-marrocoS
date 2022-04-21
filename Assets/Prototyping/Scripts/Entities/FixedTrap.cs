using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.Entities
{
    public class FixedTrap : MonoBehaviour
    {
        [field: SerializeField] public TrapType trapType { get; private set; }
        [field: SerializeField] public float value { get; private set; }
    }
}

