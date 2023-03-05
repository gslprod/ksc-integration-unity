using UnityEngine;
using UnityEngine.UI;

public class UICore : MonoBehaviour
{
    public Canvas TargetCanvas { get; private set; }
    public Transform InstantiateTransform => _instantiateTransform;
    [SerializeField] private Image _block;
    [SerializeField] private Transform _instantiateTransform;

    private void Start()
    {
        TargetCanvas = GetComponent<Canvas>();
    }

    public void SetBlock(bool isBlock)
    {
        _block.gameObject.SetActive(isBlock);
    }
}
