using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject aud;
    [SerializeField] Toggle toggle;
    [SerializeField] bool audioEnabled;

    private void OnEnable()
    {
        // Получаем состояние звука из PlayerPrefs, если оно существует
        if (PlayerPrefs.HasKey("AudioEnabled"))
        {
            audioEnabled = PlayerPrefs.GetInt("AudioEnabled") == 1;
        }
        else
        {
            // Если ключа нет, по умолчанию включаем звук
            audioEnabled = true;
            PlayerPrefs.SetInt("AudioEnabled", 1);
            PlayerPrefs.Save();
        }

        // Применяем состояние звука к аудио и тумблеру
        aud.SetActive(audioEnabled);
        toggle.isOn = audioEnabled;
    }

    public void ToggleAudio()
    {
        // Переключаем состояние звука
        audioEnabled = toggle.isOn;
        aud.SetActive(audioEnabled);

        // Сохраняем состояние звука в PlayerPrefs
        PlayerPrefs.SetInt("AudioEnabled", audioEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
