using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button Button => _button;
    public TextMeshProUGUI ButtonText => _buttonText;
    public Image BackGround => _backGround;

    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Image _backGround;


    public void SetText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        _buttonText.text = text;
    }
}
