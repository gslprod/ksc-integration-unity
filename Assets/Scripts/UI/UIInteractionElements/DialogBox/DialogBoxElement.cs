using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.DialogBox
{
    public struct DialogBoxButton
    {
        public DialogBoxButton(string text, Action onClick, bool closesDialogBox, Color color)
        {
            Text = text;
            OnClick = onClick;
            ClosesDialogBox = closesDialogBox;
            Color = color;
        }

        public string Text { get; private set; }
        public Action OnClick { get; private set; }
        public bool ClosesDialogBox { get; private set; }
        public Color Color { get; private set; }
    }

    public class DialogBoxElement : MonoBehaviour
    {
        [SerializeField] private GameObject _block;
        [SerializeField] private UIButton _buttonPrefab;
        [SerializeField] private HorizontalLayoutGroup _buttonLayoutPrefab;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private VerticalLayoutGroup _buttonLayouts;

        private List<HorizontalLayoutGroup> _buttonLayoutsList = new List<HorizontalLayoutGroup>();
        private List<UIButton> _buttons = new List<UIButton>();
        private UIFrame _uiFrame;

        private void Awake()
        {
            _uiFrame = GetComponent<UIFrame>();
        }

        public void Init(string label, string message, DialogBoxButton[] dialogBoxButtons)
        {
            SetBackgroundBlock(true);

            _label.text = label;
            _message.text = message;

            if (dialogBoxButtons.Length > 0)
            {
                for (int i = 0; i < dialogBoxButtons.Length; i++)
                {
                    var buttonObj = Instantiate(_buttonPrefab);
                    var dbButton = dialogBoxButtons[i];

                    buttonObj.Button.onClick.AddListener(() => { dbButton.OnClick?.Invoke(); });
                    if (dbButton.ClosesDialogBox)
                        buttonObj.Button.onClick.AddListener(() => { Close(); });
                    buttonObj.SetText(dbButton.Text);
                    buttonObj.BackGround.color = dbButton.Color;

                    PutButtonOnLayout(buttonObj);

                    _buttons.Add(buttonObj);
                }
                _buttonLayouts.gameObject.SetActive(true);
            }
            else
            {
                _buttonLayouts.gameObject.SetActive(false);
            }

            _uiFrame.SetActive(true);
        }

        public void Close()
        {
            _uiFrame.SetActive(false, () => { Destroy(gameObject); });
        }

        private void PutButtonOnLayout(UIButton button)
        {
            if (_buttonLayoutsList.Count > 0)
            {
                var last = _buttonLayoutsList[_buttonLayoutsList.Count - 1];

                if (last.transform.childCount <= 1)
                {
                    button.transform.SetParent(last.transform, false);
                    return;
                }
            }

            var createdLayout = Instantiate(_buttonLayoutPrefab, _buttonLayouts.transform);
            _buttonLayoutsList.Add(createdLayout);
            button.transform.SetParent(createdLayout.transform, false);
        }

        private void SetBackgroundBlock(bool isBlock)
        {
            _block.SetActive(isBlock);
        }

        public static class Factory
        {
            private const string PrefabPath = "DialogBox";

            private static DialogBoxElement _prefab;

            private static void Load()
            {
                _prefab = Resources.Load<DialogBoxElement>(PrefabPath);
            }

            public static DialogBoxElement Create(UICore target, string label, string message, DialogBoxButton[] dialogBoxButtons)
            {
                if (_prefab == null)
                    Load();

                var dialogBoxElement = Instantiate(_prefab, target.InstantiateTransform);
                dialogBoxElement.Init(label, message, dialogBoxButtons);

                return dialogBoxElement;
            }
        }
    } 
}