using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public bool dragRotateActive;
    private Vector2 lastMousePosition;
    private float rotateDir = 0f;

    void Update()
    {
        transform.position = player.transform.position;


        if (Input.GetMouseButtonDown(2))
        {
            dragRotateActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            dragRotateActive = false;
        }

        if (dragRotateActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

            rotateDir = mouseMovementDelta.x;

            lastMousePosition = Input.mousePosition;
        }

        float rotateSpeed = 100f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
}
