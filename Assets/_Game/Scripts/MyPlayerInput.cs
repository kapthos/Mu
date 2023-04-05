using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;
    public bool isAttacking;

    void Update()
    {
        if (!isAttacking && Time.timeScale != 0)
        {
            isAttacking = Input.GetMouseButtonDown(0);
        }
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
    }

    private void OnDisable()
    {
        isAttacking = false;
        HorizontalInput = 0;
        VerticalInput = 0;
    }
}
