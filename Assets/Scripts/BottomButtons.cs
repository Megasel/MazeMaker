using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottomButtons : MonoBehaviour
{
    [SerializeField] public ButtonType buttonType;
    public Transform pos;
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
    bool addShapeRewarded = false;
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
            if (tutorial.currentStep == 2 || tutorial.currentStep == 4 || tutorial.currentStep == 6 && PlayerPrefs.GetInt("tutorialCompleted") == 0) {
                buttCol.enabled = false;

            }
            else
            {
                buttCol.enabled = true;

            }
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
            print(hit.collider.name);
            
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
            currentInstance = this;
           // ExecuteAction(true); // Сохраняем ссылку на текущий объект
            Bridge.advertisement.ShowRewarded();
            addShapeRewarded = true;
        }
    }

    private void ExecuteAction(bool isRewarded)
    {
        switch (buttonType)
        {
            case ButtonType.AddShape:
                print("addShaepe");
                if(isRewarded && addShapeRewarded)
                {
                    print("addShape");
                    AddShape(isRewarded, 0);
                    addShapeRewarded = false;
                }
                else
                {
                    AddShape(isRewarded, 0);
                }
                   
                
                    
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
            foreach(AudioSource aud in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                aud.enabled = false;
            }
            Time.timeScale = 0;
        }
        if(state == RewardedState.Closed && currentInstance == this)
        {
            foreach (AudioSource aud in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                aud.enabled = true;
            }
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
                price = PlayerPrefs.GetFloat("shapesPrice", 0);
                break;
            case ButtonType.AddBall:
                price = PlayerPrefs.GetFloat("ballsPrice", 525);
                break;
            case ButtonType.Income:
                price = PlayerPrefs.GetFloat("incomePrice", 31);
                break;
        }
        priceText.text = "$" + GameManager.instance.FormatNumber(price);  // Update the UI after loading
    }

    public void AddShape(bool isRewarded, int id)
    {
        
        foreach (Cell element in cellPositions)
        {
            if (element.isEmpty)
            {
                GameObject newShape = Instantiate(shape[id], element.transform.position, Quaternion.identity, element.transform);
                newShape.GetComponent<DragAndDrop>().SnapToCell(element);
                if (!isRewarded)
                {
                    GameManager.instance.CalculatePrice(this);
                    SavePrice();  // Save price after calculation
                }
                break;
            }
        }
        if (tutorial.currentStep == 1 || tutorial.currentStep == 3 || tutorial.currentStep == 5)
        {
            tutorial.NextStep();
        }
    }
    public void AddShapeFromRewarded(bool isRewarded, int id)
    {
        
        foreach (Cell element in cellPositions)
        {
            if (element.isEmpty)
            {
                GameObject newShape = Instantiate(shape[id], element.transform.position, Quaternion.identity, element.transform);
                newShape.GetComponent<MazeElement>().level = id+ 1;
                newShape.GetComponent<DragAndDrop>().SnapToCell(element);
                if (!isRewarded)
                {
                    GameManager.instance.CalculatePrice(this);
                    SavePrice();  // Save price after calculation
                }
                break;
            }
        }
        if (tutorial.currentStep == 1 || tutorial.currentStep == 3 || tutorial.currentStep == 5)
        {
            tutorial.NextStep();
        }
    }
    public void SavePrice()
    {
        switch (buttonType)
        {
            case ButtonType.AddShape:
                PlayerPrefs.SetFloat("shapesPrice", price);
                break;
            case ButtonType.AddBall:
                PlayerPrefs.SetFloat("ballsPrice", price);
                break;
            case ButtonType.Income:
                PlayerPrefs.SetFloat("incomePrice", price);
                break;
        }
        PlayerPrefs.Save();  // Always save PlayerPrefs after a change
    }

    private bool hasBallBeenAdded = false;

    private void AddBall(bool isRewarded)
    {
        if (hasBallBeenAdded) return; // Prevent multiple executions
        hasBallBeenAdded = true;

        // Your ball-adding logic here
        Instantiate(ball, pos.position, Quaternion.identity);

        if (!isRewarded)
            GameManager.instance.CalculatePrice(this);

        hasBallBeenAdded = false; // Reset the flag after completion
    }


    void Income(bool isRewarded)
    {
        GameManager.instance.globalMultiplier += 0.01f;
        if (!isRewarded)
            GameManager.instance.CalculatePrice(this);
    }

    
}
