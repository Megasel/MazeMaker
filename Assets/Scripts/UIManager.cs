using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text multiplierText;

    public void UpdateLevel(int level)
    {
        levelText.text = "Level: " + level;
    }

    public void UpdateMultiplier(float multiplier)
    {
        multiplierText.text = "Multiplier: " + multiplier.ToString("F2");
    }
}
