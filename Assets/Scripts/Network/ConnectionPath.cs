using System.Collections.Generic;
using UnityEngine;

public class ConnectionPath : MonoBehaviour, IFilterSubElement
{
    public IConnectable First { get; private set; }
    public IConnectable Second { get; private set; }
    public IFilterElement[] Parents => _parents.ToArray();

    private List<IFilterElement> _parents = new List<IFilterElement>();
    private LineRenderer _lineRenderer;
    private float _lineWidth = 0.1f;

    private void OnDestroy()
    {
        if (First != null && Second != null)
            UnsubscribeFromConnectables();

        UnsubscribeFromFilterParents();
    }

    public void SetConnection(IConnectable first, IConnectable second)
    {
        if (first == null || second == null)
            return;

        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        First = first;
        Second = second;

        UpdatePath();
        SubscribeToConnectables();

        SubscribeIfFilterElement(first);
        SubscribeIfFilterElement(second);
        UpdateVisibility();
    }

    private void UpdatePath()
    {
        if (First == null || Second == null)
            return;

        bool firstObjFloorDetected = Physics.Raycast(new Ray(First.Position, Vector3.up), out RaycastHit firstHit, 100f, LayerMask.GetMask("Default"));
        bool secondObjFloorDetected = Physics.Raycast(new Ray(Second.Position, Vector3.up), out RaycastHit secondHit, 100f, LayerMask.GetMask("Default"));

        if (firstObjFloorDetected || secondObjFloorDetected)
        {
            Vector3 first;
            if (firstObjFloorDetected)
            {
                first = firstHit.point;
                if (secondObjFloorDetected && secondHit.point.y < firstHit.point.y)
                    first.y = secondHit.point.y - 0.05f;
                else
                    first.y -= 0.05f;
            }
            else
            {
                first = First.Position;
                first.y = secondHit.point.y - 0.05f;
            }

            Vector3 second;
            if (secondObjFloorDetected)
            {
                second = secondHit.point;
                if (firstObjFloorDetected && secondHit.point.y > firstHit.point.y)
                    second.y = firstHit.point.y - 0.05f;
                else
                    second.y -= 0.05f;
            }
            else
            {
                second = Second.Position;
                second.y = firstHit.point.y - 0.05f;
            }

            _lineRenderer.positionCount = 4;
            _lineRenderer.SetPosition(0, First.Position);
            _lineRenderer.SetPosition(1, first);
            _lineRenderer.SetPosition(2, second);
            _lineRenderer.SetPosition(3, Second.Position);
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
        }
        else
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, First.Position);
            _lineRenderer.SetPosition(1, Second.Position);
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
        }
    }

    private void UpdateVisibility()
    {
        var visible = _parents.Exists((parent) => parent.Visible);
        gameObject.SetActive(visible);
    }

    private void SubscribeIfFilterElement(object toCheck)
    {
        if (toCheck is IFilterElement filterElement)
            AddAndSubscribeToFilterParent(filterElement);
    }

    private void AddAndSubscribeToFilterParent(IFilterElement filterElement)
    {
        filterElement.OnVisibilityChanged += UpdateVisibility;
        _parents.Add(filterElement);
    }

    private void SubscribeToConnectables()
    {
        First.OnPositionChanged += UpdatePath;
        Second.OnPositionChanged += UpdatePath;
    }

    private void UnsubscribeFromConnectables()
    {
        First.OnPositionChanged -= UpdatePath;
        Second.OnPositionChanged -= UpdatePath;
    }

    private void UnsubscribeFromFilterParents()
    {
        foreach (var parent in _parents)
            parent.OnVisibilityChanged -= UpdateVisibility;
    }
}
