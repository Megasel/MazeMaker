using UnityEngine;

public class Direction : MonoBehaviour
{
    [SerializeField] private Dir direction;
    [SerializeField] private bool isBallTube;
    [SerializeField] private bool fastTube;

    private enum Dir
    {
        Left, Right, Top, Down
    }

    public void OnRayHit(BallController ball)
    {
        if (fastTube)
        {
            ball.speed = 13;
        }
        else
        {
            ball.speed = 4;
        }
        switch (direction)
        {
            case Dir.Left:
                ball.ChangeDirection(new Vector2(-1, 0));
                break;
            case Dir.Right:
                ball.ChangeDirection(new Vector2(1, 0));
                break;
            case Dir.Top:
                ball.ChangeDirection(new Vector2(0, 1));
                break;
            case Dir.Down:
                ball.ChangeDirection(new Vector2(0, -1));
                break;
        }
        if (isBallTube)
        {
           
            gameObject.SetActive(false);
        }

    }
}
