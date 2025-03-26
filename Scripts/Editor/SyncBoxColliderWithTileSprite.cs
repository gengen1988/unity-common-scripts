using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SyncBoxColliderWithTileSprite : MonoBehaviour
{
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        TryGetComponent(out boxCollider);
        TryGetComponent(out spriteRenderer);
    }

    void Update()
    {
        if (!boxCollider || !spriteRenderer || !spriteRenderer.sprite) return;
        if (!transform.hasChanged) return;
        
        Debug.Log($"transform changed, sync tile");

        transform.localScale = Vector3.one;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        boxCollider.size = spriteRenderer.size;

        // reset offset
        var pivot = spriteRenderer.sprite.pivot;
        var spriteSize = spriteRenderer.sprite.bounds.size * spriteRenderer.sprite.pixelsPerUnit;
        var anchor = Vector2.one * .5f - pivot / spriteSize;
        boxCollider.offset = anchor * spriteRenderer.size;
    }
}