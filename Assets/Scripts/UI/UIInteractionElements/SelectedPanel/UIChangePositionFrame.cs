using KSC.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.SelectedView
{
    public class UIChangePositionFrame : MonoBehaviour
    {
        [SerializeField] private Interaction _interaction;
        [SerializeField] private TMP_Dropdown _axisDropdown;
        [SerializeField] private TextMeshProUGUI _selectedDeviceText;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _minus3Button;
        [SerializeField] private Button _minus1Button;
        [SerializeField] private Button _minus03Button;
        [SerializeField] private Button _minus01Button;
        [SerializeField] private Button _3Button;
        [SerializeField] private Button _1Button;
        [SerializeField] private Button _03Button;
        [SerializeField] private Button _01Button;

        private UIFrameControl _thisFrame;

        private void Start()
        {
            _thisFrame = GetComponent<UIFrameControl>();

            UpdateVisibility();
            SubscribeToInteraction();
        }

        private void OnEnable()
        {
            SubscribeToUICallbacks();
        }

        private void OnDisable()
        {
            UnsubscribeFromUICallbacks();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInteraction();
        }

        private void ChangePositionButtonClick(float offset)
        {
            var position = _interaction.SelectedDevice.Position;
            switch (_axisDropdown.value)
            {
                case 0:
                    position.z += offset;
                    break;
                case 1:
                    position.z -= offset;
                    break;
                case 2:
                    position.x += offset;
                    break;
                case 3:
                    position.x -= offset;
                    break;
                case 4:
                    position.y += offset;
                    break;
                case 5:
                    position.y -= offset;
                    break;
            }

            _interaction.Editor.ChangeDevicePosition(_interaction.SelectedDevice, position);
        }

        #region ChangePositionsButtonsClicks

        private void Minis3ButtonClick()
            => ChangePositionButtonClick(-3);

        private void Minis1ButtonClick()
            => ChangePositionButtonClick(-1);

        private void Minis03ButtonClick()
            => ChangePositionButtonClick(-0.3f);

        private void Minis01ButtonClick()
            => ChangePositionButtonClick(-0.1f);

        private void Plus3ButtonClick()
            => ChangePositionButtonClick(3);

        private void Plus1ButtonClick()
            => ChangePositionButtonClick(1);

        private void Plus03ButtonClick()
            => ChangePositionButtonClick(0.3f);

        private void Plus01ButtonClick()
            => ChangePositionButtonClick(0.1f);

        #endregion

        private void QuitButtonClick()
        {
            _interaction.ResetState();
        }

        private void InteractionStateChanged(IInteractionState obj)
        {
            UpdateVisibility();
        }

        private void InteractionSelected(IDevice obj)
        {
            UpdateVisibility();
            if (obj == null)
                return;

            SubscribeToDevice(obj);
        }

        private void InteractionDeselected(IDevice obj)
        {
            if (obj == null)
                return;

            UnsubscribeFromDevice(obj);
        }

        private void UpdateUIName(IDevice device)
        {
            _selectedDeviceText.text = $"{device.Name.NameFormat()} ({device.Type})";
        }

        private void UpdateVisibility()
        {
            _thisFrame.SetActive(_interaction.SelectedDevice != null && _interaction.State is ChangePositionInteractionState);
        }

        private void SubscribeToInteraction()
        {
            _interaction.OnStateChanged += InteractionStateChanged;
            _interaction.OnDeselected += InteractionDeselected;
            _interaction.OnSelected += InteractionSelected;
        }

        private void UnsubscribeFromInteraction()
        {
            _interaction.OnStateChanged -= InteractionStateChanged;
            _interaction.OnDeselected -= InteractionDeselected;
            _interaction.OnSelected -= InteractionSelected;
        }

        private void SubscribeToUICallbacks()
        {
            _quitButton.onClick.AddListener(QuitButtonClick);
            _minus3Button.onClick.AddListener(Minis3ButtonClick);
            _minus1Button.onClick.AddListener(Minis1ButtonClick);
            _minus03Button.onClick.AddListener(Minis03ButtonClick);
            _minus01Button.onClick.AddListener(Minis01ButtonClick);
            _3Button.onClick.AddListener(Plus3ButtonClick);
            _1Button.onClick.AddListener(Plus1ButtonClick);
            _03Button.onClick.AddListener(Plus03ButtonClick);
            _01Button.onClick.AddListener(Plus01ButtonClick);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _quitButton.onClick.RemoveListener(QuitButtonClick);
            _minus3Button.onClick.RemoveListener(Minis3ButtonClick);
            _minus1Button.onClick.RemoveListener(Minis1ButtonClick);
            _minus03Button.onClick.RemoveListener(Minis03ButtonClick);
            _minus01Button.onClick.RemoveListener(Minis01ButtonClick);
            _3Button.onClick.RemoveListener(Plus3ButtonClick);
            _1Button.onClick.RemoveListener(Plus1ButtonClick);
            _03Button.onClick.RemoveListener(Plus03ButtonClick);
            _01Button.onClick.RemoveListener(Plus01ButtonClick);
        }

        private void SubscribeToDevice(IDevice device)
        {
            UpdateUIName(device);

            device.OnNameChanged += UpdateUIName;
        }

        private void UnsubscribeFromDevice(IDevice device)
        {
            device.OnNameChanged -= UpdateUIName;
        }
    }
}