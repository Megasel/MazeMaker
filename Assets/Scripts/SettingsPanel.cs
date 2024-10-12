using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    private void OnMouseDown()
    {
        ActivePanel();
    }
    public void ActivePanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
