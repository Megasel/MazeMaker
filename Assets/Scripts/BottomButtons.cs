using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottomButtons : MonoBehaviour
{
    [SerializeField] public ButtonType buttonType;
    [SerializeField] private Transform pos;
    [SerializeField] private GameObject ball;
    [SerializeField] private List<GameObject> shape = new List<GameObject>();
    [SerializeField] private List<Cell> cellPositions = new List<Cell>();
    [SerializeField] public float price;
    [SerializeField] public TMP_Text priceText;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject rewardButton;
    [SerializeField] private BoxCollider2D buttCol;
    [SerializeField] private BoxCollider2D rewCol;
    [SerializeField] private Tutorial tutorial;
    public enum ButtonType
    {
        AddShape,
        AddBall,
        Income
    }

    private static BottomButtons currentInstance;

    private void Start()
    {
        LoadPrice();
        Bridge.advertisement.rewardedStateChanged += OnRewardedStateChanged;
        UpdateUi();
    }

    private void Update()
    {
        if (GameManager.instance.coins >= price)
        {
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
            rewardButton.SetActive(false);
            buttCol.enabled = true;
            rewCol.enabled = false;
        }
        else
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
            rewardButton.SetActive(true);
            buttCol.enabled = false;
            if(GameManager.instance.isTutorialCompleted == 1)
                 rewCol.enabled = true;
        }
        if (Input.GetMouseButtonDown(0)) // 0 - левая кнопка мыши, 1 - правая, 2 - средняя
        {
            CheckObjectUnderMouse();
        }
    }


    void CheckObjectUnderMouse()
    {
        // Получаем позицию мыши в мировых координатах
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Выполняем Raycast для обнаружения объектов под мышью
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Проверяем, что Raycast попал в объект
        if (hit.collider != null)
        {
            // Обрабатываем взаимодействие с объектом
            Debug.Log("Clicked on object: " + hit.collider.gameObject.tag);

            
        }
        else
        {
            Debug.Log("No object clicked.");
        }
    }
    private void OnMouseDown()
    {
        Debug.Log("!");
        GameManager.instance.actions++;
        if (GameManager.instance.coins >= price)
        {
            ExecuteAction(false);
        }
        else
        {
            currentInstance = this; // Сохраняем ссылку на текущий объект
            Bridge.advertisement.ShowRewarded();
        }
    }

    private void ExecuteAction(bool isRewarded)
    {
        switch (buttonType)
        {
            case ButtonType.AddShape:
                AddShape(isRewarded,0);
                break;
            case ButtonType.AddBall:
                if(GameManager.instance.isTutorialCompleted == 1)
                {
                    AddBall(isRewarded);
                }
                
                break;
            case ButtonType.Income:
                if (GameManager.instance.isTutorialCompleted == 1)
                {
                    Income(isRewarded);
                }
                    
                break;
        }
    }

    private void OnRewardedStateChanged(RewardedState state)
    {
        if(state == RewardedState.Opened && currentInstance == this)
        {
            Time.timeScale = 0;
        }
        if(state == RewardedState.Closed && currentInstance == this)
        {
            Time.timeScale = 1;
        }
        if (state == RewardedState.Rewarded && currentInstance == this)
        {
            ExecuteAction(true);
        }
    }

    public void UpdateUi()
    {
        priceText.text = "$" + GameManager.instance.FormatNumber(price);
    }

    private void LoadPrice()
    {
        switch (buttonType)
        {
            case ButtonType.AddShape:
                price = GameManager.instance.shapesPrice;
                break;
            case ButtonType.AddBall:
                price = GameManager.instance.ballsPrice;
                break;
            case ButtonType.Income:
                price = GameManager.instance.incomePrice;
                break;
        }
    }

    public void AddShape(bool isRewarded,int id)
    {
        foreach (Cell element in cellPositions)
        {
            if (element.isEmpty)
            {
                GameObject newShape = Instantiate(shape[id], element.transform.position, Quaternion.identity, element.transform);
                newShape.GetComponent<DragAndDrop>().SnapToCell(element);
                if (!isRewarded)
                    GameManager.instance.CalculatePrice(this);
                break;
            }
        }
        if(tutorial.currentStep == 1 || tutorial.currentStep == 3 || tutorial.currentStep == 5)
        {
            tutorial.NextStep();
        }
    }

    void AddBall(bool isRewarded)
    {
        Instantiate(ball, pos.position, Quaternion.identity);
        if (!isRewarded)
            GameManager.instance.CalculatePrice(this);
    }

    void Income(bool isRewarded)
    {
        GameManager.instance.globalMultiplier += 0.01f;
        if (!isRewarded)
            GameManager.instance.CalculatePrice(this);
    }

    
}
