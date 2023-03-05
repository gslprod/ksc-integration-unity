using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevicePrefab : MonoBehaviour
{
    public Image Icon => _icon;
    public TextMeshProUGUI Text => _text;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
}
