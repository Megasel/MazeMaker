
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : MonoBehaviour
{
    private void OnEnable()
    {
        print(PlayerPrefs.GetInt("Level"));
        if(PlayerPrefs.HasKey("Level") && PlayerPrefs.GetInt("Level") != 0)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("Level", 0));
        }
       

    }
}
