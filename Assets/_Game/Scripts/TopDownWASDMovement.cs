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

    //Variaveis movimento
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;

    Vector3 testeMovimentoRelativo;
    bool isMovementPressed;
    bool isRunPressed = false;
    public float speed = 5f;
    public float runMultiplier = 3f;

    //Variaveis de localização do Mouse
    Ray ray;
    RaycastHit hit;
    public float turnSpeed = 7f;
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
        playerInput.CharacterControls.Shield.started += onShieldStance;
        playerInput.CharacterControls.Shield.canceled += onShieldStance;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 / maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2.0f * maxJumpHeight) / timeToApex;
    }

    void onShieldStance(InputAction.CallbackContext context)
    {
        isShieldUp = context.ReadValueAsButton();
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
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
        Quaternion rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
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

    void handleJump()
    {
        if (!isJumping && controller.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
            currentRunMovement.y = initialJumpVelocity * 0.5f;
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

        // Vector3 forward = Camera.main.transform.forward;
        // Vector3 right = Camera.main.transform.right;
        // forward.y = 0;
        // right.y = 0;
        // forward = forward.normalized;
        // right = right.normalized;

        // Vector3 forwardRelative = currentMovement.z * forward;
        // Vector3 rightRelative = currentMovement.x * right;

        // testeMovimentoRelativo = forwardRelative + rightRelative;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleShieldStance()
    {
        if (controller.isGrounded && isShieldUp)
        {
            animator.SetBool("isShielding", true);
        }
        else
        {
            animator.SetBool("isShielding", false);
        }
    }

    void handleAnimation()
    {
        Vector3 inputVector = (Vector3.forward * currentMovement.z) + (Vector3.right * currentMovement.x);

        Vector3 animationVector = transform.InverseTransformDirection(inputVector);

        var VelocityX = animationVector.x;
        var VelocityZ = animationVector.z;

        animator.SetFloat("horizontalMovement", VelocityX);
        animator.SetFloat("verticalMovement", VelocityZ);
    }

    void Update()
    {
        handleAnimation();
        mouseRotation();

        if (isRunPressed)
        {
            controller.Move(currentRunMovement * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(currentMovement * speed * Time.deltaTime);
        }
        handleShieldStance();
        handleGravity();
        handleJump();
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
