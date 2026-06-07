using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FixTiledScaling : MonoBehaviour
{
    public BoxCollider2D bc;
    void LateUpdate()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();

        if (sr.drawMode != SpriteDrawMode.Tiled)
            return;

        Vector3 scale = transform.localScale;

        // Apply scale to tiled size
        sr.size = new Vector2(
            sr.size.x * scale.x,
            sr.size.y * scale.y
        );

        // Reset transform scale
        transform.localScale = Vector3.one;
        if (bc != null)
        {
            bc.size = sr.size;
        }
    }
}