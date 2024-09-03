using UnityEngine;
using DG.Tweening;

public class BallController : MonoBehaviour
{
    public float speed = 15f;
    public Vector2 direction;
    public bool inTube = true;
    public float rayLength = 2f; // Длина луча
    private Collider2D lastHitCollider;
    [SerializeField] Sprite[] sprites;
    private bool isRedirecting = false; // Флаг для предотвращения многократных срабатываний
    Rigidbody2D rb;

    [SerializeField] ParticleSystem particleSystem;

    // Свойство для доступа к текущему направлению мячика
    public Vector2 CurrentDirection
    {
        get { return direction; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

    private void Update()
    {
        // Перемещаем мячик в соответствии с направлением и скоростью
        Vector2 newPosition = rb.position + direction * speed * Time.deltaTime;

        // Move the Rigidbody2D to the new position
        rb.MovePosition(newPosition);

        // Отображаем луч в направлении движения
        DrawDirectionRay();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Rewarded"))
            return;
        if (other != lastHitCollider)
        {
            lastHitCollider = other;

            if (other.CompareTag("Direction"))
            {
                if (other.TryGetComponent<Direction>(out Direction directionComponent))
                {
                    directionComponent.OnRayHit(this);
                }
            }
            else if (other.CompareTag("Gipotenuza") || other.CompareTag("Katet") || other.CompareTag("Petal"))
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
            }
            else if (other.CompareTag("Ball"))
            {
                if (other.transform.parent == null || !other.transform.parent.CompareTag("MazeElement"))
                {
                    direction *= -1;
                    ParticleSystem part = Instantiate(particleSystem, transform.position, Quaternion.identity);
                    part.transform.position = transform.position;
                    part.Play();
                    Destroy(part.gameObject, 2);
                }
                else
                {
                    direction *= -1;
                }
            }
        }
    }
}
