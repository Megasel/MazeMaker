using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InTubeElement : MonoBehaviour
{
    [SerializeField] GameObject moneyTextEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<BallController>(out BallController ballContrl))
        {
            float addedCoins = 3 * GameManager.instance.globalMultiplier;


            MazeElement mazeElement = GetComponent<MazeElement>();
            

            GameObject textContainer = new GameObject("TextContainer");
            textContainer.transform.position =transform.position;

            GameObject text = Instantiate(moneyTextEffect, Vector3.zero, Quaternion.identity, textContainer.transform);
            TMP_Text textComponent = text.GetComponent<TMP_Text>();
            textComponent.text = FormatNumber(addedCoins);
            textComponent.rectTransform.anchoredPosition = new Vector2(0, textComponent.rectTransform.position.y);
            Destroy(textContainer, 1.5f);

            GameManager.instance.coins += addedCoins;
            GameManager.instance.UpdateUi();
        }
    }
    public string FormatNumber(float number)
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
            return number.ToString("0.00");
        }
    }
}
