using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.Controllers;

namespace Prototyping.Scripts.ScriptableObjects
{
    public enum CursorType
    {
        Default,
        GrabbingTrap,
        CanPlaceTrap,
        CanTriggerTrap
    }
    
    [CreateAssetMenu(fileName = "New Cursor Animation", menuName = "Cursor Animation")]
    public class CursorAnimation : ScriptableObject
    {
        public CursorType cursorType;
        public List<Texture2D> textures;
        public float frameRate;
        public Vector2Int offset;
    }
}