using InstantGamesBridge;
using TMPro;
using UnityEngine;

public class TmpLocalisation : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [Space(20)]
    [TextArea] [SerializeField] private string _ru;
    [TextArea] [SerializeField] private string _en;
    [TextArea] [SerializeField] private string _tr;

    private void OnValidate()
    {
        if (_text== null && TryGetComponent(out TMP_Text text))
        {
            _text = text;
            _ru = _text.text;
        }
    }

    private void Start()
    {
        switch (Bridge.platform.language)
        {
            case "ru":
                _text.text = _ru; 
                break;
            case "en":
                _text.text = _en;
                break;
            case "tr":
                _text.text = _tr;
                break;
            default:
                _text.text = "";
                _text.enabled = false;
                break;
        }
    }
}
