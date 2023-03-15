using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMax = 25f;
    [SerializeField] private float lowerYMin = 5f;
    [SerializeField] private float lowerYMax = 25f;
    private Vector3 followOffset;
    public bool isHeldDown = false;
    float zoomSpeed = 2f;
    float zoomAmount = 2f;
    Vector3 zoomDir;

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

    void HandleCameraZoomStyles()
    {
        Vector3 zoomDir = followOffset.normalized;
        float zoomAmount = 2f;

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

        float zoomSpeed = 2f;
        Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
    }

    void Update()
    {
        HoldButton();
        HandleCameraZoomStyles();
    }
}