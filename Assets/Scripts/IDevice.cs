using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public interface IDevice
{
    event Action<IDevice> OnNameChanged;

    string Type { get; }
    string Name { get; }
    Vector3 Position { get; }
    GameObject GameObject { get; }

    void ChangePosition(Vector3 newPosition);
    void ChangeName(string newName);
    void SetTextMesh(TextMeshProUGUI textMesh);
    void SetImage(Image image);
}
