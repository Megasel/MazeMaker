using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;

    private void Start()
    {
        

       
        if (PlayerPrefs.HasKey("AudioEnabled"))
        {
            bool audioEnabled = PlayerPrefs.GetInt("AudioEnabled") == 1;
            ToggleAudioState(audioEnabled);
        }
    }
    private void OnMouseDown()
    {
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        PlayerPrefs.SetInt("SettingsPanelVisible", isActive ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void Toggle()
    {
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        PlayerPrefs.SetInt("SettingsPanelVisible", isActive ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleAudio(AudioSource aud)
    {
        bool audioEnabled = !aud.enabled;
        aud.enabled = audioEnabled;

        PlayerPrefs.SetInt("AudioEnabled", audioEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ToggleAudioState(bool enabled)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource aud in audioSources)
        {
            aud.enabled = enabled;
        }
    }
}
