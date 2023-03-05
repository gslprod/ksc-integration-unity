using TMPro;
using UnityEngine;

namespace UIGroups.ViewEdit
{
	public class UIViewDropdown : MonoBehaviour
	{
        private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }

        private void OnEnable()
        {
            SubscribeToDropdown();
        }

        private void OnDisable()
        {
            UnsubscribeFromDropdown();
        }

        private void ValueChanged(int value)
        {
            ViewFilter.SetMode((ViewFilter.ViewMode)value);
        }

        private void SubscribeToDropdown()
        {
            _dropdown.onValueChanged.AddListener(ValueChanged);
        }

        private void UnsubscribeFromDropdown()
        {
            _dropdown.onValueChanged.RemoveListener(ValueChanged);
        }
    } 
}
