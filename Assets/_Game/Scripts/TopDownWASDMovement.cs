using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownWASDMovement : MonoBehaviour
{
    //Referencias
    PlayerInput playerInput;
    CharacterController controller;
    Animator animator;
    [SerializeField] GameObject miraTeste;

    //Variaveis movimento
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isWalking = false;

    bool isMovementPressed;
    bool isRunPressed = false;
    [SerializeField] float speed;
    float runMultiplier = 2f;
    float rotationFactorPerFrame = 1.0f;

    [SerializeField] float regularSpeed = 5f;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float stunned = 0f;

    //Variaveis de localização do Mouse
    Ray ray;
    RaycastHit hit;
    float turnSpeed = 7f;
    bool freezeY = true;

    //Variaveis de Gravidade
    float gravity = -9.81f;
    float groundedGravity = -0.05f;

    //Variaveis de pulo
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1.2f;
    float maxJumpTime = 0.6f;
    bool isJumping = false;
    int isJumpingHash;
    bool isJumpingAnimating;

    //Variaveis de Shield
    bool isShieldUp = false;

    public bool movimentoMouse = true;
    public bool movimentoTarget = false;
    bool isMouseActive = false;

    bool isAttacking = false;


    private void Awake()
    {
        playerInput = new PlayerInput();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
        speed = 5f;
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 / maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2.0f * maxJumpHeight) / timeToApex;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if (controller.isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
            isJumpingAnimating = false;
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

    void changeMoveStyle()
    {
        isMouseActive = animator.GetBool("isMouseActive");
        if (movimentoTarget)
        {
            animator.SetBool("isMouseActive", false);
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                movimentoMouse = !movimentoMouse;
                movimentoTarget = !movimentoTarget;
            }
            handleRotation();
        }
        else if (movimentoMouse)
        {
            animator.SetBool("isMouseActive", true);
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                movimentoMouse = !movimentoMouse;
                movimentoTarget = !movimentoTarget;
            }
            mouseRotation();
        }
    }

    void handleJump()
    {
        if (!isJumping && controller.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.43f;
            currentRunMovement.y = initialJumpVelocity * 0.43f;
        }
        else if (!isJumpPressed && isJumping && controller.isGrounded)
        {
            isJumping = false;
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

    void mouseRotation()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit))
        {
            return;
        }
        Vector3 pos = hit.point - transform.position;

        if (freezeY)
        {
            pos.y = 0;
        }

        miraTeste.transform.position = pos;

        Quaternion rot = Quaternion.LookRotation(miraTeste.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, turnSpeed);
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
            animator.SetFloat("speedAnim", 0.45f);
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
        if (Input.GetMouseButtonDown(0) && controller.isGrounded)
        {
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

    void DashTest(){
        if(Input.GetKeyDown(KeyCode.LeftAlt)){
            // speed = speed * 5f;
            animator.SetBool("isDashing",true);
        }
        if(Input.GetKeyUp(KeyCode.LeftAlt)){
            // speed = regularSpeed;
        }
    }

    void Update()
    {
        handleAnimation();
        changeMoveStyle();
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
        handleJump();
    }

    void FootR(){
        
    }
    void FootL(){

    }
    void Hit(){

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