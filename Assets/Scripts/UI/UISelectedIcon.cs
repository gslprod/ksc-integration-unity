using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectedIcon : MonoBehaviour
{
    [SerializeField] private Interaction _interaction;

    private Camera _camera;
    private IDevice _target;

    private void Awake()
    {
        _camera = GetComponentInParent<Camera>();
    }

    private void Start()
    {
        _interaction.OnSelected += ActiveChanged;
        gameObject.SetActive(false);
    }

    private void ActiveChanged(IDevice obj)
    {
        _target = obj;
        bool active = obj != null;
        if (active)
            UpdatePosition();
        gameObject.SetActive(obj != null);
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var newPosition = _camera.WorldToScreenPoint(_target.Position);
        if (_target != null && newPosition.z > 0)
            transform.position = newPosition;
    }

    private void OnDestroy()
    {
        _interaction.OnSelected -= ActiveChanged;
    }
}
