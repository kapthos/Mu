using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMax = 25f;
    [SerializeField] private float lowerYMin = 2f;
    [SerializeField] private float lowerYMax = 25f;
    private Vector3 followOffset;
    public bool isHeldDown = false;
    float zoomSpeed = 2.5f;
    float zoomAmount = 2f;
    Vector3 zoomDir;
    bool DragRotateActive;
    Vector3 lastMousePosition;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    void HoldButton()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isHeldDown = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isHeldDown = false;
        }
    }

    void HandleCameraRotation()
    {
        float rotateDir = 0f;
        float rotateSpeed = 100f;

        if (Input.GetMouseButtonDown(1))
        {
            DragRotateActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            DragRotateActive = false;
        }

        if (DragRotateActive)
        {
            Vector3 mouseMovementDelta = Input.mousePosition - lastMousePosition;
            rotateDir = mouseMovementDelta.x;
            lastMousePosition = Input.mousePosition;
        }

        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }

    void HandleCameraZoomStyles()
    {
        Vector3 zoomDir = followOffset.normalized;

        if (Input.mouseScrollDelta.y > 0)
        {
            if (isHeldDown)
            {
                followOffset.y -= zoomAmount;
                followOffset.y = Mathf.Clamp(followOffset.y, lowerYMin, lowerYMax);
            }
            else if (!isHeldDown)
            {
                followOffset -= zoomAmount * zoomDir;
                if (followOffset.magnitude < followOffsetMin)
                {
                    followOffset = zoomDir * followOffsetMin;
                }
                if (followOffset.magnitude > followOffsetMax)
                {
                    followOffset = zoomDir * followOffsetMax;
                }
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (isHeldDown)
            {
                followOffset.y += zoomAmount;
                followOffset.y = Mathf.Clamp(followOffset.y, lowerYMin, lowerYMax);
            }
            else if (!isHeldDown)
            {
                followOffset += zoomAmount * zoomDir;
                if (followOffset.magnitude < followOffsetMin)
                {
                    followOffset = zoomDir * followOffsetMin;
                }
                if (followOffset.magnitude > followOffsetMax)
                {
                    followOffset = zoomDir * followOffsetMax;
                }
            }
        }

        Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
    }

    void Update()
    {
        HandleCameraRotation();
        HoldButton();
        HandleCameraZoomStyles();
    }
}