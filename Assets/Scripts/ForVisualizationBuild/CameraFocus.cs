using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFocus : MonoBehaviour
{
    [SerializeField]
    Camera _camera;
    [SerializeField]
    Transform cameraFocus;
    private void Update()
    {
        _camera.transform.LookAt(cameraFocus);
        _camera.transform.RotateAround(cameraFocus.position, Vector3.up, 15f * Time.deltaTime);
    }
}
