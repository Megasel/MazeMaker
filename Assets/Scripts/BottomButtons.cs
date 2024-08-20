using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottomButtons : MonoBehaviour
{
    [SerializeField] public ButtonType buttonType;
    [SerializeField] private Transform pos;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject shape;
    [SerializeField] private List<Cell> cellPositions = new List<Cell>();
    [SerializeField] public float price;
    [SerializeField] public TMP_Text priceText;

    [SerializeField] SpriteRenderer spriteRenderer;

    public enum ButtonType
    {
        AddShape,
        AddBall,
        Income
    }

    private void Start()
    {
        LoadPrice();

        UpdateUi();
    }
    private void Update()
    {
        if(GameManager.instance.coins >= price)
        {
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }
        else
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
        }
    }
    private void OnMouseDown()
    {
        GameManager.instance.actions++;
        if (GameManager.instance.coins >= price)
        {
            switch (buttonType)
            {
                case ButtonType.AddShape:
                    AddShape();
                    break;
                case ButtonType.AddBall:
                    AddBall();
                    break;
                case ButtonType.Income:
                    Income();
                    break;
            }
        }
        else
        {
        }
        GameManager.instance.ShowInterstitial();
    }

    public void UpdateUi()
    {
        priceText.text = "$" + FormatNumber(price);
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

    void AddShape()
    {
        foreach (Cell element in cellPositions)
        {
            if (element.isEmpty)
            {
                GameObject newShape = Instantiate(shape, element.transform.position, Quaternion.identity, element.transform);
                newShape.GetComponent<DragAndDrop>().SnapToCell(element);
                GameManager.instance.CalculatePrice(this);
                break;
            }
        }
    }

    void AddBall()
    {
        Instantiate(ball, pos.position, Quaternion.identity);
        GameManager.instance.CalculatePrice(this);
    }

    void Income()
    {
        GameManager.instance.globalMultiplier += 0.01f;
        GameManager.instance.CalculatePrice(this);
    }

    private string FormatNumber(float number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.0") + "M";
           
        }
            
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("0.0") + "K";
        }
            
        else
        {
            
            return number.ToString("0.0");
        }
            
    }
}
