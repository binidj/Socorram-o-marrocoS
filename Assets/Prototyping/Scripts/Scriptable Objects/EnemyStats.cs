using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Enemy Stats")]
    public class EnemyStats : ScriptableObject
    {
        public float speed;
        public float health;
    }
}