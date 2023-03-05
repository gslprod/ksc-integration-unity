using System;
using UnityEngine;

public class UIFrame : MonoBehaviour
{
    public bool Active => _active;

    [SerializeField] private bool _active = false;
    [SerializeField] private GameObject _frameGameObject;

    private void Awake()
    {
        _frameGameObject.SetActive(_active);
    }

    public void SetActive(bool isActive)
    {
        if (isActive == _active)
            return;

        _active = isActive;
        if (_active)
            Appear();
        else
            Disappear();
    }

    public void SetActive(bool isActive, Action onComplete)
    {
        if (isActive == _active)
            return;

        if (onComplete == null)
        {
            SetActive(isActive);
            return;
        }

        _active = isActive;
        if (_active)
            Appear(onComplete);
        else
            Disappear(onComplete);
    }

    private void Appear()
    {
        _frameGameObject.SetActive(true);
    }

    private void Appear(Action onComplete)
    {
        _frameGameObject.SetActive(true);
        onComplete();
    }

    private void Disappear()
    {
        _frameGameObject.SetActive(false);
    }

    private void Disappear(Action onComplete)
    {
        _frameGameObject.SetActive(false);
        onComplete();
    }
}
