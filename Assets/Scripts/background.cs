using UnityEngine;

public class background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {

        Screen.SetResolution(1080, 1920,FullScreenMode.FullScreenWindow);
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    private void Update()
    {
        StretchSpriteToScreen();
    }

    void StretchSpriteToScreen()
    {
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Sprite not assigned to the SpriteRenderer component.");
            return;
        }

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        transform.localScale = new Vector3(
            screenWidth / spriteRenderer.sprite.bounds.size.x,
            screenHeight / spriteRenderer.sprite.bounds.size.y,
            1f
        );
    }
}
