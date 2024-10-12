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
        // �������� ��������� ����� �� PlayerPrefs, ���� ��� ����������
        if (PlayerPrefs.HasKey("AudioEnabled"))
        {
            audioEnabled = PlayerPrefs.GetInt("AudioEnabled") == 1;
        }
        else
        {
            // ���� ����� ���, �� ��������� �������� ����
            audioEnabled = true;
            PlayerPrefs.SetInt("AudioEnabled", 1);
            PlayerPrefs.Save();
        }

        // ��������� ��������� ����� � ����� � ��������
        aud.SetActive(audioEnabled);
        toggle.isOn = audioEnabled;
    }

    public void ToggleAudio()
    {
        // ����������� ��������� �����
        audioEnabled = toggle.isOn;
        aud.SetActive(audioEnabled);

        // ��������� ��������� ����� � PlayerPrefs
        PlayerPrefs.SetInt("AudioEnabled", audioEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
