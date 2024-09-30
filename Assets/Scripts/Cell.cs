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

        // ����������� ������� Y � ��������� �� 0 �� 1
        float normalizedY = Mathf.InverseLerp(0f, 5.76f, yPosition);

        // �������� ������������ ��� ��������� V �� 0.8 �� 1
        float v = Mathf.Lerp(0.9f, 1f, normalizedY);

        // �������� ������� ���� �������
        Color currentColor = spriteRenderer.color;

        // ��������� ���� �� RGB � HSV
        Color.RGBToHSV(currentColor, out float h, out float s, out float _);

        // ������� ����� ���� � ���������� ��������� V
        Color newColor = Color.HSVToRGB(h, s, v);

        // ��������� ����� ���� � �������
        spriteRenderer.color = newColor;
    }
}
