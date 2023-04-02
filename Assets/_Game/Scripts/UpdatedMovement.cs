using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpdatedMovement : MonoBehaviour
{
    //Referencias
    PlayerInput playerInput;
    CharacterController controller;
    Animator animator;
    SelectionManager selectionManager;
    [SerializeField] Camera mainCamera;

    //Variaveis movimento
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isWalking = false;

    bool isMovementPressed;
    bool isRunPressed = false;
    float speed;
    float runMultiplier = 1.7f;
    float rotationFactorPerFrame = 0.1f;

    float regularSpeed = 7f;
    float walkSpeed = 2f;
    float stunned = 0f;

    //Variaveis de Gravidade
    float gravity = -9.81f;
    float groundedGravity = -0.05f;

    //Variaveis de Shield
    bool isAttacking = false;

    float dashSpeed = 5.5f;
    float dashTime = 0.1f;

    private void Awake()
    {
        playerInput = new PlayerInput();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        selectionManager = GetComponent<SelectionManager>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        speed = 7f;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f;
        float fallMultiplier = 2.0f;
        if (controller.isGrounded)
        {
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
    }

    private void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        }
    }

    void handleAnimation()
    {
        isWalking = animator.GetBool("isWalking");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
        Vector3 inputVector = (Vector3.forward * currentMovement.z) + (Vector3.right * currentMovement.x);

        Vector3 animationVector = transform.InverseTransformDirection(inputVector);

        var VelocityX = animationVector.x;
        var VelocityZ = animationVector.z;

        animator.SetFloat("horizontalMovement", VelocityX);
        animator.SetFloat("verticalMovement", VelocityZ);
    }

    void handleShieldStance()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isShielding", true);
            animator.SetFloat("speedAnim", 0.25f);
            speed = walkSpeed;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("isShielding", false);
            speed = regularSpeed;
        }
    }

    void handleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycast);

            Vector3 directionToFace = raycast.point - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToFace);

            isAttacking = true;
            animator.SetBool("isAttacking", true);
            speed = stunned;
        }
    }

    void AttackEnds()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        speed = regularSpeed;
    }

    void DashTest()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            animator.SetBool("isWalking", false);
            controller.Move(currentMovement * speed * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }

    void Update()
    {
        handleAnimation();
        handleRotation();
        if (isRunPressed)
        {
            controller.Move(currentRunMovement * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(currentMovement * speed * Time.deltaTime);
        }
        handleShieldStance();
        handleAttack();
        DashTest();
        handleGravity();
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

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}