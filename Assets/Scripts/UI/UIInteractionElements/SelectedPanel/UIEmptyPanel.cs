using UnityEngine;

namespace UIGroups.SelectedView
{
    public class UIEmptyPanel : MonoBehaviour
    {
        [SerializeField] private Interaction _interaction;

        private UIFrameControl _thisFrame;

        private void Start()
        {
            _thisFrame = GetComponent<UIFrameControl>();
            UpdateVisibility();
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void InteractionStateChanged(IInteractionState obj)
        {
            UpdateVisibility();
        }

        private void InteractionActiveChanged(IDevice obj)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            _thisFrame.SetActive(_interaction.SelectedDevice == null);
        }

        private void Subscribe()
        {
            _interaction.OnStateChanged += InteractionStateChanged;
            _interaction.OnSelected += InteractionActiveChanged;
        }

        private void Unsubscribe()
        {
            _interaction.OnStateChanged -= InteractionStateChanged;
            _interaction.OnSelected -= InteractionActiveChanged;
        }
    }
}