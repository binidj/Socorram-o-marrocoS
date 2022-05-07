using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Controllers
{    
    public class CursorController : MonoBehaviour
    {
        public static CursorController Instance { get; private set; }
        public CursorType cursorType { get{ return cursorAnimation.cursorType; } }
        [SerializeField] private List<CursorAnimation> cursorAnimations;
        private CursorAnimation cursorAnimation;
        private int textureCount;
        private int currentFrame;
        private float frameTimer;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetActiveCursorType(CursorType.Default);
        }

        private void Update()
        {
            frameTimer -= Time.deltaTime;
            if (frameTimer <= 0)
            {
                frameTimer = cursorAnimation.frameRate;
                currentFrame = (currentFrame + 1) % textureCount;
                Cursor.SetCursor(cursorAnimation.textures[currentFrame], cursorAnimation.offset, CursorMode.Auto);
            }
        }

        private void SetActiveCursorAnimation(CursorAnimation cursorAnimation)
        {
            this.cursorAnimation = cursorAnimation;
            currentFrame = 0;
            textureCount = cursorAnimation.textures.Count;
            frameTimer = cursorAnimation.frameRate;
            Cursor.SetCursor(cursorAnimation.textures[currentFrame], cursorAnimation.offset, CursorMode.Auto);
        }

        public void SetActiveCursorType(CursorType cursorType)
        {
            SetActiveCursorAnimation(GetCursorAnimation(cursorType));     
        }

        private CursorAnimation GetCursorAnimation(CursorType cursorType)
        {
            foreach (CursorAnimation cursorAnimation in cursorAnimations)
            {
                if (cursorAnimation.cursorType == cursorType)
                {
                    return cursorAnimation;
                }
            }
            return null;
        }
    }
}


