using KSC.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.SelectedView
{
    public class UIObjectInteractionPanel : MonoBehaviour
    {
        [SerializeField] private UserInput _input;
        [SerializeField] private Interaction _interaction;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TextMeshProUGUI _selectedDeviceText;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _positionEditorButton;
        [SerializeField] private Button _connectionsEditorButton;
        [SerializeField] private Button _kscButton;
        [SerializeField] private GameObject _nameBlockInfoGroup;
        [Header("Filter")]
        [SerializeField] private GameObject _filterGroup;
        [SerializeField] private TMP_Dropdown _filterModeDropdown;
        [SerializeField] private TMP_InputField _floorInputField;
        [SerializeField] private Button _autoFloorButton;

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

        private void DeleteButtonClick()
        {
            _interaction.DestroySelectedDevice();
        }

        private void ConnectionsEditorButtonClick()
        {
            _interaction.ChangeState(new ConnectionsInteractionState());
        }

        private void PositionEditorButtonClick()
        {
            _interaction.ChangeState(new ChangePositionInteractionState());
        }

        private void KSCButtonClick()
        {
            _interaction.ChangeState(new KSCInteractionState());
        }

        private void AnyInputFieldSelect(string value)
        {
            _input.SetLock(this);
        }

        private void AnyInputFieldDeselect(string value)
        {
            _input.RemoveLock(this);
        }

        private void NameInputFieldDeselect(string value)
        {
            _interaction.Editor.ChangeDeviceName(_interaction.SelectedDevice, value);
        }

        private void FilterModeDropdownValueChanged(int value)
        {
            if (!(_interaction.SelectedDevice is IFilterElement filterElement))
                return;

            filterElement.SetVisibilityMode((IFilterElement.VisibilityMode)value);
        }

        private void FloorInputFieldValueChanged(string value)
        {
            if (!(_interaction.SelectedDevice is IFilterElement filterElement))
                return;

            if (!int.TryParse(_floorInputField.text, out int number))
                return;

            filterElement.SetFloor(number);
        }

        private void AutoFloorButtonClick()
        {
            if (!(_interaction.SelectedDevice is IFilterElement filterElement))
                return;

            _interaction.Editor.AutoCalculateFloor(filterElement);
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
            if (obj is IFilterElement filterElement)
                SubscribeToFilterElement(filterElement); 
            if (obj is IKSCAgent kscAgent)
                SubscribeToKSCAgent(kscAgent);
            else
                UpdateUIByAgentObject(null);
        }

        private void InteractionDeselected(IDevice obj)
        {
            if (obj == null)
                return;

            UnsubscribeFromDevice(obj);
            if (obj is IFilterElement filterElement)
                UnsubscribeFromFilterElement(filterElement);
            if (obj is IKSCAgent kscAgent)
                UnsubscribeFromKSCAgent(kscAgent);
        }

        private void UpdateUIName(IDevice device)
        {
            _selectedDeviceText.text = $"{device.Name.NameFormat()} ({device.Type})";
            _nameInputField.SetTextWithoutNotify(device.Name);
        }

        private void UpdateUIFilterElementMode(IFilterElement filterElement)
        {
            _filterModeDropdown.SetValueWithoutNotify((int)filterElement.Mode);
        }

        private void UpdateUIFilterElementFloor(IFilterElement filterElement)
        {
            _floorInputField.SetTextWithoutNotify(filterElement.Floor.ToString());
        }

        private void UpdateUIByAgentObject(IKSCAgent kscAgent)
        {
            var isNameInputBlocked = kscAgent != null && kscAgent.UseKSCAgentName;

            UpdateNameInputFieldAviability(isNameInputBlocked);
            UpdateNameBlockInfoVisibility(isNameInputBlocked);
        }

        private void UpdateNameInputFieldAviability(bool isBlocked)
        {
            _nameInputField.interactable = !isBlocked;
        }

        private void UpdateNameBlockInfoVisibility(bool isBlocked)
        {
            _nameBlockInfoGroup.SetActive(isBlocked);
        }

        private void UpdateVisibility()
        {
            bool isActive = _interaction.SelectedDevice != null && _interaction.State is ObjectInteractionState;
            _thisFrame.SetActive(isActive);

            if (!isActive)
                return;

            _connectionsEditorButton.gameObject.SetActive(_interaction.SelectedDevice is IConnectable);
            _filterGroup.SetActive(_interaction.SelectedDevice is IFilterElement);
            _kscButton.gameObject.SetActive(_interaction.SelectedDevice is IKSCAgent);
        }

        private void SubscribeToInteraction()
        {
            _interaction.OnStateChanged += InteractionStateChanged;
            _interaction.OnSelected += InteractionSelected;
            _interaction.OnDeselected += InteractionDeselected;
        }

        private void UnsubscribeFromInteraction()
        {
            _interaction.OnStateChanged -= InteractionStateChanged;
            _interaction.OnSelected -= InteractionSelected;
            _interaction.OnDeselected -= InteractionDeselected;
        }

        private void SubscribeToUICallbacks()
        {
            _nameInputField.onSelect.AddListener(AnyInputFieldSelect);
            _nameInputField.onDeselect.AddListener(NameInputFieldDeselect);
            _nameInputField.onDeselect.AddListener(AnyInputFieldDeselect);
            _deleteButton.onClick.AddListener(DeleteButtonClick);
            _positionEditorButton.onClick.AddListener(PositionEditorButtonClick);
            _connectionsEditorButton.onClick.AddListener(ConnectionsEditorButtonClick);
            _kscButton.onClick.AddListener(KSCButtonClick);

            _filterModeDropdown.onValueChanged.AddListener(FilterModeDropdownValueChanged);
            _floorInputField.onValueChanged.AddListener(FloorInputFieldValueChanged);
            _floorInputField.onSelect.AddListener(AnyInputFieldSelect);
            _floorInputField.onDeselect.AddListener(AnyInputFieldDeselect);
            _autoFloorButton.onClick.AddListener(AutoFloorButtonClick);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _nameInputField.onSelect.RemoveListener(AnyInputFieldSelect);
            _nameInputField.onDeselect.RemoveListener(NameInputFieldDeselect);
            _nameInputField.onDeselect.RemoveListener(AnyInputFieldDeselect);
            _deleteButton.onClick.RemoveListener(DeleteButtonClick);
            _positionEditorButton.onClick.RemoveListener(PositionEditorButtonClick);
            _connectionsEditorButton.onClick.RemoveListener(ConnectionsEditorButtonClick);
            _kscButton.onClick.RemoveListener(KSCButtonClick);

            _filterModeDropdown.onValueChanged.RemoveListener(FilterModeDropdownValueChanged);
            _floorInputField.onValueChanged.RemoveListener(FloorInputFieldValueChanged);
            _floorInputField.onSelect.RemoveListener(AnyInputFieldSelect);
            _floorInputField.onDeselect.RemoveListener(AnyInputFieldDeselect);
            _autoFloorButton.onClick.RemoveListener(AutoFloorButtonClick);
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

        private void SubscribeToFilterElement(IFilterElement filterElement)
        {
            UpdateUIFilterElementFloor(filterElement);
            UpdateUIFilterElementMode(filterElement);

            filterElement.OnFloorChanged += UpdateUIFilterElementFloor;
            filterElement.OnModeChanged += UpdateUIFilterElementMode;
        }

        private void UnsubscribeFromFilterElement(IFilterElement filterElement)
        {
            filterElement.OnFloorChanged -= UpdateUIFilterElementFloor;
            filterElement.OnModeChanged -= UpdateUIFilterElementMode;
        }

        private void SubscribeToKSCAgent(IKSCAgent kscAgent)
        {
            UpdateUIByAgentObject(kscAgent);

            kscAgent.OnKSCNameUsingChanged += UpdateUIByAgentObject;
        }

        private void UnsubscribeFromKSCAgent(IKSCAgent kscAgent)
        {
            kscAgent.OnKSCNameUsingChanged -= UpdateUIByAgentObject;

        }
    }
}