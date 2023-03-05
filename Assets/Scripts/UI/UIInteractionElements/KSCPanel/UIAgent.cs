using KSC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.KSC
{
    public class UIAgent : MonoBehaviour
    {
        public event Action<UIAgent> OnButtonClick;
        public event Action<UIAgent> OnDispose;

        public Agent Agent { get; private set; }

        [SerializeField] private TextMeshProUGUI _ipText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Image _backGround;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
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
            if (Agent != null)
                UnsubscribeFromAgent();

            OnDispose?.Invoke(this);
        }

        public void Init(Agent agent)
        {
            if (Agent != null)
                throw new InvalidOperationException();

            Agent = agent;

            UpdateInfo();
            SubscribeToAgent();
        }

        public void UpdateInfo()
        {
            _ipText.text = Agent.IP;
            _nameText.text = $"{Agent.Name} ({Agent.Group})";
            _statusText.text = Agent.Status.StatusEnumToText();
        }

        public void ChangeBackGroundColor(Color newColor)
        {
            _backGround.color = newColor;
        }

        private void ButtonClick()
            => OnButtonClick?.Invoke(this);

        private void SubscribeToUICallbacks()
        {
            _button.onClick.AddListener(ButtonClick);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _button.onClick.RemoveListener(ButtonClick);
        }

        private void SubscribeToAgent()
        {
            Agent.OnInfoUpdated += AgentInfoUpdated;
            Agent.OnDispose += AgentDispose;
        }

        private void UnsubscribeFromAgent()
        {
            Agent.OnInfoUpdated -= AgentInfoUpdated;
            Agent.OnDispose -= AgentDispose;
        }

        private void AgentInfoUpdated(Agent agent)
        {
            UpdateInfo();
        }

        private void AgentDispose(Agent agent)
        {
            Destroy(gameObject);
        }
    }
}
