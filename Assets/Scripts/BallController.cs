using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

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
    [SerializeField] private List<MazeElement> mazeElements;
    [SerializeField] private float timeLoop;
    [SerializeField] private float startTimeLoop;
    Coroutine coroutine;
    // Свойство для доступа к текущему направлению мячика
    public Vector2 CurrentDirection
    {
        get { return direction; }
    }

    // Границы прямоугольника
    private Vector2 minBounds; // Левая нижняя точка
    private Vector2 maxBounds; // Правая верхняя точка

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

    private void OnEnable()
    {
        minBounds = GameManager.instance.minBounds.position;
        maxBounds = GameManager.instance.maxBounds.position;
    }

    private void Update()
    {
        // Перемещаем мячик в соответствии с направлением и скоростью
        Vector2 newPosition = rb.position + direction * speed * Time.deltaTime;

        // Ограничиваем позицию в пределах прямоугольника
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        // Перемещаем Rigidbody2D к новой позиции
        rb.MovePosition(newPosition);

        // Отображаем луч в направлении движения
        DrawDirectionRay();
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        // Отладка: вывод нового направления до нормализации

        direction = new Vector2(
            Mathf.RoundToInt(newDirection.x), // Направление по X может быть -1, 0 или 1
            Mathf.RoundToInt(newDirection.y)  // Направление по Y может быть -1, 0 или 1
        ).normalized;

        // Отладка: вывод нормализованного направления
    }

    // Метод для выравнивания мяча по сетке
    private void AlignToGrid()
    {
        // Если мяч движется по оси X, корректируем его по оси Y
        if (direction.x != 0)
        {
            Transform nearestPointY = FindNearestPointByY(transform.position, GameManager.instance.verticalPoints);
            transform.position = new Vector2(transform.position.x, nearestPointY.position.y);
        }
        // Если мяч движется по оси Y, корректируем его по оси X
        else if (direction.y != 0)
        {
            Transform nearestPointX = FindNearestPointByX(transform.position, GameManager.instance.horizontalPoints);
            transform.position = new Vector2(nearestPointX.position.x, transform.position.y);
        }
    }

    // Метод для нахождения ближайшей точки по оси X
    private Transform FindNearestPointByX(Vector2 currentPosition, List<Transform> points)
    {
        Transform nearestPoint = points[0];
        float minDistance = Mathf.Abs(currentPosition.x - points[0].position.x);

        // Отладка: вывод начальной ближайшей точки и её расстояния

        foreach (Transform point in points)
        {
            float distance = Mathf.Abs(currentPosition.x - point.position.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = point;

                // Отладка: вывод новой ближайшей точки, если она найдена
            }
        }

        return nearestPoint;
    }

    // Метод для нахождения ближайшей точки по оси Y
    private Transform FindNearestPointByY(Vector2 currentPosition, List<Transform> points)
    {
        Transform nearestPoint = points[0];
        float minDistance = Mathf.Abs(currentPosition.y - points[0].position.y);

        // Отладка: вывод начальной ближайшей точки и её расстояния

        foreach (Transform point in points)
        {
            float distance = Mathf.Abs(currentPosition.y - point.position.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = point;

                // Отладка: вывод новой ближайшей точки, если она найдена
            }
        }

        return nearestPoint;
    }

    private void DrawDirectionRay()
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(direction.x, direction.y, 0) * rayLength;

        Debug.DrawRay(start, end - start, Color.red);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rewarded"))
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
                    print("checkloop");
                    CheckLoop(mazeElement);
                    if (other.CompareTag("Gipotenuza"))
                    {
                        
                        if (mazeElement.gameObject.tag != "Respawn")
                        {
                            mazeElement.Reflect(direction, this, true);
                        }
                        mazeElement.AddMoney(false, mazeElement.transform);
                        AlignToGrid();
                    }
                    else if (other.CompareTag("Katet"))
                    {
                        mazeElement.Reflect(direction, this, false);
                        AlignToGrid();
                    }
                    else if (other.CompareTag("Petal"))
                    {
                        CheckLoop(mazeElement);
                        mazeElement.transform.localScale = new Vector3(1,1,1);
                        mazeElement.AddMoney(true, mazeElement.transform);
                        if (!mazeElement.isScaling)
                        {
                            mazeElement.isScaling = true;
                            mazeElement.gameObject.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f).OnComplete(() => mazeElement.isScaling = false);

                        }
                        else
                        {
                            mazeElement.transform.localScale = new Vector3(1, 1, 1);
                            mazeElement.isScaling = false;
                        }
                           
                    }

                    // Корректируем мяч по сетке после столкновения
                    
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

                // Корректируем мяч по сетке после столкновения
                AlignToGrid();
            }
        }
    }
    void CheckLoop(MazeElement mazeElement)
    {
        print("checkloop");
        // Add the element if it's not already in the list
        if (!mazeElements.Contains(mazeElement))
        {
            mazeElements.Add(mazeElement);
            ResetTimer(); // Reset the timer when a new element is added
        }

        // Start the coroutine if it's not already running
        if (coroutine == null)
        {
            coroutine = StartCoroutine(Loop());
        }
    }

    IEnumerator Loop()
    {
        // Continue looping while the ball is in the tube
        while (!inTube)
        {
            // Decrease the timer each frame
            timeLoop -= Time.deltaTime;

            // If the timer reaches 0, destroy the ball object and stop the coroutine
            if (timeLoop <= 0)
            {
                GameManager.instance.AddBall();
                StopCoroutine(coroutine);
                coroutine = null;  // Clear the coroutine reference
                ClearMazeElements();  // Clear the list of maze elements
                Destroy(gameObject);  // Destroy the ball object
                yield break;  // Exit the coroutine
            }

            // Wait until the next frame
            yield return null;
        }
        ResetTimer();
        ClearMazeElements();
        StopCoroutine(coroutine);
        coroutine = null;
    }

    // Method to reset the timer manually
    public void ResetTimer()
    {
        timeLoop = startTimeLoop;  // Reset the timer to its starting value
        Debug.Log("Timer reset!");
    }

    // Method to clear the list of maze elements
    public void ClearMazeElements()
    {
        mazeElements.Clear();  // Clear the list to free up memory
        Debug.Log("Maze elements cleared!");
    }

    // Optional: Call this method when the object is disabled or destroyed to stop coroutine safely
    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);  // Stop the coroutine if it's running
            coroutine = null;  // Clear the coroutine reference
        }

        ClearMazeElements();  // Clear the maze elements list
    }


}
