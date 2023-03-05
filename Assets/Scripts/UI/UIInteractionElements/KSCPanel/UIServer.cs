using KSC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.KSC
{
    public class UIServer : MonoBehaviour
    {
        public event Action<UIServer> OnButtonClick;
        public event Action<UIServer> OnDispose;

        public Server Server { get; private set; }

        [SerializeField] private TextMeshProUGUI _ipText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _userText;
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
            if (Server != null)
                UnsubscribeFromServer();

            OnDispose?.Invoke(this);
        }

        public void Init(Server server)
        {
            if (Server != null)
                throw new InvalidOperationException();

            Server = server;
            
            UpdateInfo();
            SubscribeToServer();
        }

        public void UpdateInfo()
        {
            _ipText.text = Server.IPAddress.RemoveHostFromIPAddress();
            _nameText.text = Server.ServerName;
            _userText.text = Server.UserName;
        }

        public void ChangeBackGroundColor(Color newColor)
        {
            _backGround.color = newColor;
        }

        private void Dispose()
            => Destroy(gameObject);

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

        private void SubscribeToServer()
        {
            Server.OnInfoUpdated += ServerInfoUpdated;
            Server.OnDispose += ServerDispose;
        }

        private void UnsubscribeFromServer()
        {
            Server.OnInfoUpdated -= ServerInfoUpdated;
            Server.OnDispose -= ServerDispose;
        }

        private void ServerInfoUpdated(Server server)
        {
            UpdateInfo();
        }

        private void ServerDispose(Server server)
        {
            Dispose();
        }
    }
}