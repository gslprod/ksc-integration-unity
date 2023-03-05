using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace UIGroups.CreatePanel
{
    public class UICreateObject : MonoBehaviour
    {
        [SerializeField] private DevicesEditor _devicesEditor;
        [SerializeField] private UserInput _user;
        [SerializeField] private UIFrameControl _createObjectsFrame;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _cancelButton;

        private TMP_Dropdown _possibleDevices;
        private List<Type> _currentTypes;

        private void Awake()
        {
            _possibleDevices = GetComponentInChildren<TMP_Dropdown>();

            InitDropdown();
        }

        private void OnEnable()
        {
            SubcribeToButtons();
        }

        private void OnDisable()
        {
            UnsubcribeFromButtons();
        }

        private void SubcribeToButtons()
        {
            _createButton.onClick.AddListener(CreateButtonClick);
            _cancelButton.onClick.AddListener(CancelButtonClick);
        }

        private void UnsubcribeFromButtons()
        {
            _createButton.onClick.RemoveListener(CreateButtonClick);
            _cancelButton.onClick.RemoveListener(CancelButtonClick);
        }

        private void InitDropdown()
        {
            _possibleDevices.ClearOptions();

            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var possibleObject in MainValues.AllObjectTypes)
            {
                options.Add(new TMP_Dropdown.OptionData(
                    possibleObject.GetProperty("StaticType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .GetValue(null, null)
                    .ToString()));
            }

            _possibleDevices.AddOptions(options);
            _currentTypes = new List<Type>(MainValues.AllObjectTypes);
        }

        private void CreateButtonClick()
        {
            _devicesEditor.CreateDevice(_currentTypes[_possibleDevices.value], _user.GetLookPosition(2f));
        }

        private void CancelButtonClick()
        {
            _createObjectsFrame.SetActive(false);
        }
    }
}