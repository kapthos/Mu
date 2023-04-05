using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterController _controller;
    Animator _animator;
    MyPlayerInput _myPlayerInput;
    [SerializeField] Camera mainCamera;

    //Movement
    [SerializeField] float moveSpeed;
    Vector3 cameraRelativeMovement;
    Vector3 moveDir;

    //Camera Relative Movement
    Vector3 forward;
    Vector3 right;
    Vector3 forwardRelativeToCamera;
    Vector3 rightRelativeToCamera;
    Vector3 yAxisFiltered;

    //Gravity
    float _gravity = -9.81f;
    float _verticalVelocity;
    [SerializeField] float gravityMultiplier;

    // Dash
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;


    //State Machine
    public enum CharacterState
    {
        Normal, Attacking
    }
    public CharacterState currentState;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _myPlayerInput = GetComponent<MyPlayerInput>();
    }

    void CalculatePlayerMovement()
    {
        if (_myPlayerInput.isAttacking && _controller.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }

        forward = mainCamera.transform.forward;
        right = mainCamera.transform.right;
        forwardRelativeToCamera = _myPlayerInput.VerticalInput * forward;
        rightRelativeToCamera = _myPlayerInput.HorizontalInput * right;
        cameraRelativeMovement = (forwardRelativeToCamera + rightRelativeToCamera).normalized;

        yAxisFiltered = new Vector3(cameraRelativeMovement.x, 0, cameraRelativeMovement.z);

        if (cameraRelativeMovement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(yAxisFiltered);
        }
        yAxisFiltered *= moveSpeed * Time.deltaTime;
        moveDir = yAxisFiltered;
    }
    void HandleGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0.0f)
        {
            _verticalVelocity = -0.1f;
        }
        else
        {
            _verticalVelocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
        moveDir.y = _verticalVelocity;
    }
    void HandleAnimations()
    {
        _animator.SetFloat("Speed", cameraRelativeMovement.magnitude);
    }
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                CalculatePlayerMovement();
                break;
            case CharacterState.Attacking:

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit raycast);

                Vector3 directionToFace = raycast.point - transform.position;
                transform.rotation = Quaternion.LookRotation(directionToFace);

                moveDir = Vector3.zero;
                break;
        }

        HandleAnimations();
        HandleGravity();
        _controller.Move(moveDir);
    }

    void DashTest()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetFloat("Speed", 0);
            StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            _controller.Move(moveDir * moveSpeed * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }
    void SwitchStateTo(CharacterState newState)
    {
        // Clear Cache
        _myPlayerInput.isAttacking = false;

        //Exiting State
        switch (currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                break;
        }
        //Entering State
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                _animator.SetTrigger("Attacking");
                break;
        }
        currentState = newState;
    }
    private void Update()
    {
        DashTest();
    }

    void FootR()
    {

    }
    void FootL()
    {

    }
    void Hit()
    {

    }
    void AttackEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
}
