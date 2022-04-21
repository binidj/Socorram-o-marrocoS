using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Level Config")]
    public class LevelConfig : ScriptableObject
    {
        public Vector3Int startPosition;
        public Vector3Int endPosition;
        public int tilesLimit;
    }
}