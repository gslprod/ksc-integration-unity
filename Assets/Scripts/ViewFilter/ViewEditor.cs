using System.Collections.Generic;
using UnityEngine;

public class ViewEditor : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _firstFloorObjects;
    [SerializeField] private List<MeshRenderer> _secondFloorObjects;
    [SerializeField] private List<MeshRenderer> _thirdFloorObjects;
    [SerializeField] private List<MeshRenderer> _fourthFloorObjects;

    private List<MeshRenderer> _allObjects = new List<MeshRenderer>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            _allObjects.Add(transform.GetChild(i).GetComponent<MeshRenderer>());

        UpdateView();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void UpdateView()
    {
        if (ViewFilter.Mode != ViewFilter.ViewMode.All)
            _allObjects.ForEach(obj => obj.enabled = false);

        switch (ViewFilter.Mode)
        {
            case ViewFilter.ViewMode.All:
                _allObjects.ForEach(obj => obj.enabled = true);
                break;
            case ViewFilter.ViewMode.OnlyFirstFloor:
                _firstFloorObjects.ForEach(obj => obj.enabled = true);
                break;
            case ViewFilter.ViewMode.OnlySecondFloor:
                _secondFloorObjects.ForEach(obj => obj.enabled = true);
                break;
            case ViewFilter.ViewMode.OnlyThirdFloor:
                _thirdFloorObjects.ForEach(obj => obj.enabled = true);
                break;
            case ViewFilter.ViewMode.OnlyFourthFloor:
                _fourthFloorObjects.ForEach(obj => obj.enabled = true);
                break;
        }
    }

    private void Subscribe()
    {
        ViewFilter.OnModeChanged += UpdateView;
    }

    private void Unsubscribe()
    {
        ViewFilter.OnModeChanged -= UpdateView;
    }
}
