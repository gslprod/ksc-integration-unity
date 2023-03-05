using KSC;
using System;
using TMPro;
using UIGroups.DialogBox;
using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.KSC
{
    public class UIKSCAuthPanel : MonoBehaviour
    {
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _connectButton;
        [SerializeField] private UIFrameControl _kscAuthFrame;
        [SerializeField] private UIKSCPanel _kscPanel;
        [Header("Inputs")]
        [SerializeField] private TMP_InputField _ipInputField;
        [SerializeField] private TMP_Dropdown _sslDropdown;
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private TMP_InputField _domainInputField;
        [SerializeField] private TMP_InputField _passwordInputField;

        private UICore _uiCore;

        private void Awake()
        {
            _uiCore = GetComponentInParent<UICore>();
        }

        private void OnEnable()
        {
            SubscribeToButtons();
        }

        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }

        private void CancelClick()
        {
            _kscAuthFrame.SetActive(false);
        }

        private async void ConnectClick()
        {
            var buttons = new DialogBoxButton[]
            {
                new DialogBoxButton("������������� ������� (�� �������������)", () =>{ }, true, new Color(1, 0, 0))
            };
            var loadingDialogBox = DialogBoxElement.Factory.Create(_uiCore, "����������� � �������",
                "����������, ���������. ��� ����� ������ ��������� �����. ������ ���� ��������� ������������� ����� ���������� ��������.\n" +
                "��������� ������ ���� ������� �� �������������, ������ � ������ ��������� �� ������ ������� ���.",
                buttons);

            try
            {
                await Server.ConnectAndReturnAsync(_ipInputField.text, SSLDropdownValueToBool(),
                    _userNameInputField.text, _domainInputField.text, _passwordInputField.text);

                _kscAuthFrame.SetActive(false);
            }
            catch (Exception e)
            {
                var errorDBbuttons = new DialogBoxButton[]
                {
                    new DialogBoxButton("�������", () =>{ }, true, new Color(0.5f, 0.5f, 0.5f))
                };
                var errorDialogBox = DialogBoxElement.Factory.Create(_uiCore, "���� ����������� � �������",
                    $"�������: {e.GetType().Name} - {e.Message}",
                    errorDBbuttons);
                throw;
            }
            finally
            {
                loadingDialogBox.Close();
            }
        }

        private bool SSLDropdownValueToBool()
            => _sslDropdown.value == 1;

        private void SubscribeToButtons()
        {
            _cancelButton.onClick.AddListener(CancelClick);
            _connectButton.onClick.AddListener(ConnectClick);
        }

        private void UnsubscribeFromButtons()
        {
            _cancelButton.onClick.RemoveListener(CancelClick);
            _connectButton.onClick.RemoveListener(ConnectClick);
        }
    }
}