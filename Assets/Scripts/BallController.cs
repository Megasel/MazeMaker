using UnityEngine;
using DG;
using DG.Tweening;
using Unity.Burst.CompilerServices;
public class BallController : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;
    public bool inTube = true;
    public float rayLength = 2f; // Длина луча
    private Collider2D lastHitCollider;
    [SerializeField] Sprite[] sprites;
    private bool isRedirecting = false; // Флаг для предотвращения многократных срабатываний

    // Свойство для доступа к текущему направлению мячика
    public Vector2 CurrentDirection
    {
        get { return direction; }
    }
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
    private void Update()
    {
        // Перемещаем мячик в соответствии с направлением и скоростью
        transform.Translate(direction * speed * Time.deltaTime);
       
        // Отображаем луч в направлении движения
        DrawDirectionRay();

        // Проверка столкновения луча с коллайдером
        if (!isRedirecting)
        {
            CheckRayCollision();
        }
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized; // Нормализуем новое направление
    }

    private void DrawDirectionRay()
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(direction.x, direction.y, 0) * rayLength;

        Debug.DrawRay(start, end - start, Color.red);
    }

    private void CheckRayCollision()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Ball"));

        Vector2 start = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, rayLength, layerMask);

        if (hit.collider != null && hit.collider != lastHitCollider)
        {
            lastHitCollider = hit.collider; 

            if (hit.collider.TryGetComponent<Direction>(out Direction directionComponent) && hit.collider.CompareTag("Direction"))
            {
                directionComponent.OnRayHit(this);
            }
            else if (hit.collider.TryGetComponent<CheckBall>(out CheckBall checkBallComponent))
            {
                checkBallComponent.OnRayHit();
            }
           
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Ball")
        {

            if (other != null && other != lastHitCollider)
            {
                if (other.transform.parent.TryGetComponent<MazeElement>(out MazeElement mazeElement))
                {
                    if (other.CompareTag("Gipotenuza"))
                    {

                        if (mazeElement.gameObject.tag != "Respawn")
                        {

                            mazeElement.Reflect(direction, this, true);
                        }

                        mazeElement.AddMoney(false, mazeElement.transform);
                    }
                    else if (other.CompareTag("Katet"))
                    {
                        mazeElement.Reflect(direction, this, false);
                    }
                    else if (other.CompareTag("Petal"))
                    {

                        mazeElement.AddMoney(true, mazeElement.transform);
                        mazeElement.gameObject.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f);
                    }
                }

                GameManager.instance.SaveGame();
            }
        }
        else
        {
            print("p");
            direction *= -1;
            
        }
    }
        
}





