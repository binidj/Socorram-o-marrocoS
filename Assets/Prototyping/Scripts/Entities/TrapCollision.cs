using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    public bool isColliding {get; set;} = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        isColliding = false;
    }
}
