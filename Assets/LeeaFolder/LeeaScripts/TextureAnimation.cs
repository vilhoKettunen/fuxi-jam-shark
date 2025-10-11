using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    public int uvAnimationTileX = 24; // Number of columns in the sprite sheet
    public int uvAnimationTileY = 1; // Number of rows in the sprite sheet
    public float framesPerSecond = 10f;
    private Renderer rend;
    private int index;
    private Vector2 size;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        size = new Vector2(1.0f / uvAnimationTileX, 1.0f / uvAnimationTileY);
    }

    void Update()
    {
        index = (int)(Time.time * framesPerSecond) % (uvAnimationTileX * uvAnimationTileY);
        offset = new Vector2((index % uvAnimationTileX) * size.x, 1.0f - size.y - (index / uvAnimationTileX) * size.y);
        rend.material.SetTextureOffset("_MainTex", offset);
        rend.material.SetTextureScale("_MainTex", size);
    }
}