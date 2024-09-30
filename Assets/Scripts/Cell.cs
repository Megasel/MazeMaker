using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isEmpty = true;
    SpriteRenderer spriteRenderer;
    public bool isTutorialCell = false;
    public int tutorialStep;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MazeElement"))
        {
            isEmpty = false;
        }
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ColorGradient();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MazeElement"))
        {
            isEmpty = true;

        }
    }
    private void ColorGradient()
    {
        float yPosition = transform.localPosition.y;

        // Нормализуем позицию Y в диапазоне от 0 до 1
        float normalizedY = Mathf.InverseLerp(0f, 5.76f, yPosition);

        // Линейная интерполяция для получения V от 0.8 до 1
        float v = Mathf.Lerp(0.9f, 1f, normalizedY);

        // Получаем текущий цвет спрайта
        Color currentColor = spriteRenderer.color;

        // Переводим цвет из RGB в HSV
        Color.RGBToHSV(currentColor, out float h, out float s, out float _);

        // Создаем новый цвет с измененным значением V
        Color newColor = Color.HSVToRGB(h, s, v);

        // Применяем новый цвет к спрайту
        spriteRenderer.color = newColor;
    }
}
