using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    public bool isColliding {get; set;} = false;

    private void OntriggerEnter2D(Collider2D other) 
    {
        Debug.Log(gameObject + " enter");
        isColliding = true;
    }

    private void OntriggerStay2D(Collider2D other) 
    {
        Debug.Log(gameObject + " stay");
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Debug.Log(gameObject + " exit");
        isColliding = false;
    }
}
