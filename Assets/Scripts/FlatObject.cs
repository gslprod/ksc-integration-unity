using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatObject : MonoBehaviour
{
    private Camera _userCamera;

    private void Awake()
    {
        _userCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(-(_userCamera.transform.position - transform.position));
    }
}
