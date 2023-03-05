using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFrameControl : MonoBehaviour
{
    public event Action<UIFrameControl> OnSubFrameSwitched;

    public UIFrame Target { get { if (_target == null) _target = GetComponent<UIFrame>(); return _target; } }
    public bool ControlEnabled => _controlThisFrame;

    [SerializeField] private bool _controlThisFrame = true;
    [SerializeField] private List<UIFrameControl> _subFrames;

    private UIFrame _target;

    private void Awake()
    {
        if (_controlThisFrame && _target == null)
            _target = GetComponent<UIFrame>();
    }

    public void SwitchSubFrame(UIFrameControl newFrame)
    {
        if (!newFrame.ControlEnabled)
            return;

        if (_subFrames.Count == 0 || !_subFrames.Contains(newFrame))
            throw new ArgumentException();

        foreach (var subFrame in _subFrames)
        {
            if (subFrame == newFrame)
                OnSubFrameSwitched?.Invoke(newFrame);

            subFrame.SetActive(subFrame == newFrame);
        }
    }

    public void SetActive(bool isActive)
    {
        if (_controlThisFrame)
            _target.SetActive(isActive);
    }
}
