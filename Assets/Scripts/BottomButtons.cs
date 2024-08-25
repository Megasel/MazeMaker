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
            rewCol.enabled = true;
        }
    }

    private void OnMouseDown()
    {
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
                AddBall(isRewarded);
                break;
            case ButtonType.Income:
                Income(isRewarded);
                break;
        }
    }

    private void OnRewardedStateChanged(RewardedState state)
    {
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
