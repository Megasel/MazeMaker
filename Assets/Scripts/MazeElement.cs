using TMPro;
using UnityEngine;

public class MazeElement : MonoBehaviour
{
    public int level;
    public int multiplier;
    public enum ElementType { Triangle, Rectangle }
    public ElementType type;
    [SerializeField] GameObject moneyTextEffect;
    public enum TriangleOrientation { BottomLeft, BottomRight, TopLeft, TopRight }
    public TriangleOrientation triangleOrientation;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            BallController ball = collision.collider.GetComponent<BallController>();
            if (ball != null)
            {
                RedirectBall(ball, collision.contacts[0].normal);
            }
        }
    }

    public void RedirectBall(BallController ball, Vector2 normal)
    {
        Vector2 incomingDirection = ball.CurrentDirection;
        if (type == ElementType.Triangle)
        {
            
            Reflect(incomingDirection,ball , true);

            
        }
        else if (type == ElementType.Rectangle)
        {

            Reflect(incomingDirection, ball, false);
        }
    }
    public void AddMoney(bool isPetal, Transform pos)
    {
        float addedCoins = GameManager.instance.globalMultiplier * Mathf.Pow(2.4f, level - 1);


        MazeElement mazeElement = GetComponent<MazeElement>();
        Vector3 localPosition = new Vector3(0, 0, 0); // Ќапример, это локальна€ позици€ внутри MazeElement

        Vector3 worldPosition = mazeElement.transform.TransformPoint(localPosition);
        
        GameObject textContainer = new GameObject("TextContainer");
        textContainer.transform.position = worldPosition;

        GameObject text = Instantiate(moneyTextEffect, Vector3.zero, Quaternion.identity, textContainer.transform);
        TMP_Text textComponent = text.GetComponent<TMP_Text>();
        textComponent.text = GameManager.instance.FormatNumber(addedCoins);
        textComponent.rectTransform.anchoredPosition = new Vector2(0, textComponent.rectTransform.position.y);
        Destroy(textContainer, 1.5f);

        GameManager.instance.coins += addedCoins;
        GameManager.instance.UpdateUi();




    }
    public void Reflect(Vector2 incomingDirection, BallController ball, bool isGipotinuze)
    {
        Vector2 newDirection = Vector2.zero;


        if (isGipotinuze)
        {
            switch (triangleOrientation)
            {
                case TriangleOrientation.BottomLeft:
                    
                    if (incomingDirection == new Vector2(1, 0))
                    {
                        ball.direction = new Vector2(0, -1);
                    }
                    else
                    {
                        ball.direction = new Vector2(-1, 0);
                    }
                    break;

                case TriangleOrientation.BottomRight:
                    print(incomingDirection);
                    if (incomingDirection == new Vector2(-1, 0))
                    {
                        ball.direction = new Vector2(0, -1);
                    }
                    else
                    {
                        ball.direction = new Vector2(1, 0);
                    }
                    break;

                case TriangleOrientation.TopLeft:
                    print(incomingDirection);
                    if (incomingDirection == new Vector2(1, 0))
                    {
                        ball.direction = new Vector2(0, 1);
                    }
                    else
                    {
                        ball.direction = new Vector2(-1, 0);
                    }
                    break;

                case TriangleOrientation.TopRight:
                    print(incomingDirection);
                    if (incomingDirection == new Vector2(-1, 0))
                    {
                        ball.direction = new Vector2(0, 1);
                    }
                    else
                    {
                        ball.direction = new Vector2(1, 0);
                    }
                    break;
            }
        }
        

        else
        {

            ball.direction *= -1;
                    
        }


    }
}
