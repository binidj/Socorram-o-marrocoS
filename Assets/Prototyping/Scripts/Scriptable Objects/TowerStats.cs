using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Tower Stats", menuName = "Tower Stats")]
    public class TowerStats : ScriptableObject
    {
        public float damagePerSecond;
    }
}