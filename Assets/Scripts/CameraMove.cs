using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private Transform target;
    private Vector3 cameraOffset;

    [Header("Zoom")]
    [SerializeField]
    private float zoomSpeed = 1.0f;
    private float minZoom = 2.0f;
    private float maxZoom = 10.0f;

    [Header("Orbit")]
    [SerializeField]
    private float orbitSpeed = 1.0f;
    private float mouseX;
    private float mouseY;
    [SerializeField]
    private float smoothSpeed = 0.125f;


    private void Start()
    {
        this.cameraOffset = this.transform.position - this.target.position;
    }

    private void Update()
    {
        AddZoom();
        AddOrbit();
        AddOrbitKeyboard();
    }

    private void AddZoom()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        float zoomAmount = scroll * this.zoomSpeed;
        float newDistance = Mathf.Clamp(this.cameraOffset.magnitude + zoomAmount, minZoom, maxZoom);
        this.cameraOffset = this.cameraOffset.normalized * newDistance;
    }

    private void AddOrbitKeyboard()
    {
        this.mouseX += Input.GetAxisRaw("Horizontal") * this.orbitSpeed;
    }

    private void AddOrbit()
    {
        this.mouseX += Input.GetAxisRaw("Mouse X") * this.orbitSpeed;
        this.mouseY -= Input.GetAxisRaw("Mouse Y") * this.orbitSpeed;
        this.mouseY = Mathf.Clamp(this.mouseY, -35, 60);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(this.mouseY, this.mouseX, 0);
        Vector3 rotatedOffset = rotation * this.cameraOffset;

        Vector3 newPosition = target.position + rotatedOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, newPosition, this.smoothSpeed); 
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
