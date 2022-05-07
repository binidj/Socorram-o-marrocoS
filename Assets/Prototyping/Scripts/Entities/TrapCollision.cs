using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    public bool isColliding {get; set;} = false;
    SpriteRenderer spriteRenderer;
    LayerMask placed;
    Coroutine flashing = null;
    [SerializeField] private float flashInDuration = 0.5f;
    [SerializeField] private float flashOutDuration = 0.5f;
    [SerializeField] private float maxAlpha = 0.4f;
    bool isFlashing = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        placed = LayerMask.NameToLayer("PlacedTrap");
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        isColliding = true;
    }

    private void Update()
    {
        if (gameObject.layer != placed.value) return;
        
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = placed;
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(gameObject.GetComponent<Collider2D>(), filter, colliders);

        if (colliders.Count > 0 && !isFlashing)
        {
            flashing = StartCoroutine(Flashing());
            isFlashing = true;
        }

        if (colliders.Count == 0 && isFlashing)
        {
            StopCoroutine(flashing);
            Color32 color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            isFlashing = false;
        }
            
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        isColliding = false;
    }

    private IEnumerator Flashing()
    {
        while (true)
        {
            for (float time = 0f; time <= flashInDuration; time += Time.deltaTime)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(0, maxAlpha, time / flashInDuration);
                spriteRenderer.color = color;
                yield return null;
            }

            for (float time = 0f; time <= flashOutDuration; time += Time.deltaTime)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(0, maxAlpha, 1f - (time / flashInDuration));
                spriteRenderer.color = color;
                yield return null;
            }
        }
    }
}
