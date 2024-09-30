
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private int price;
   
    public void OnMouseUp()
    {
        print("next");
        if (GameManager.instance.coins >= price)
        {
            if (PlayerPrefs.HasKey("Level"))
            {
                GoToNextLevel(PlayerPrefs.GetInt("Level")+ 1);
            }

            else
            {
                Debug.Log("1");

                GoToNextLevel(1);
            }

            
        }
    }



    private void GoToNextLevel(int sceneId)
    {
        // ��������� ������� �������� incomelevel � incomeprice
        int savedIncomeLevel = PlayerPrefs.GetInt("incomelevel", 1);
        float savedIncomePrice = PlayerPrefs.GetFloat("incomePrice", 31);
        float savedCoins = PlayerPrefs.GetFloat("coins") - price;
        // ������� ��� ����������� ������
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("coins", savedCoins);
        // ������������ ������ incomelevel � incomeprice
        PlayerPrefs.SetInt("incomelevel", savedIncomeLevel);
        PlayerPrefs.SetFloat("incomePrice", savedIncomePrice);
        PlayerPrefs.SetInt("tutorialCompleted", 1);
        PlayerPrefs.SetInt("Level", sceneId);
        // ��������� ���������
        PlayerPrefs.Save();
        Debug.Log(sceneId);
        print("goScene");
        SceneManager.LoadScene(sceneId);
    }

}
