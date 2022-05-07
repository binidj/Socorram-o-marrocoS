using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using Prototyping.Scripts.Controllers;

public class CursorObject : MonoBehaviour
{
    [SerializeField] private CursorType cursorType;
    private void OnMouseEnter() 
    {
        CursorController.Instance.SetActiveCursorType(cursorType);
    }

    private void OnMouseExit() 
    {
        CursorController.Instance.SetActiveCursorType(CursorType.Default);    
    }
}
