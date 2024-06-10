using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterLocomotion : MonoBehaviour
{
    [Tooltip("Assign animator if you would like. We are using 2d blendtree")]
    [SerializeField] Animator animator;
    [Tooltip("Character controller is a built in component in unity. Feel free to use rigidbody or changing transform directly")]
    [SerializeField] CharacterController characterController;
    [Tooltip("How fast the player walks")]
    [SerializeField] float walkSpeed = 3f;
    [Tooltip("If you would like separate visual from player assign something else here")]
    [SerializeField] Transform characterVisual;
    [Tooltip("Turn this off if you want to separate movement and aiming")]
    [SerializeField] bool lookToMovementDirection = true;
    [Tooltip("Feel free to assign other joysticks here")]
    [SerializeField] FixedJoystick moveJoystick;
    [Tooltip("Self explanatory. After this magnitude player will move ")]
    [SerializeField] float movementThreshold = 0.1f;
    [Tooltip("Jump height for the character")]
    [SerializeField] float jumpHeight = 2f;
    [Tooltip("Gravity modifier for the character")]
    [SerializeField] float gravity = -9.8f;

    private float verticalSpeed = 0f;
    private bool isGrounded = true;
    private bool jumpRequest = false;

    [Header("Animation variables")]
    [Tooltip("This will turn rotation towards the joystick direction")]
    [SerializeField] bool canStrafe = false;
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string forwardAnimationVar = "Forward";
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string strafeAnimationVar = "Strafe";

    float mag;
    Transform camTransform;
    Vector3 fwd, right;
    Vector3 input, move;
    Vector3 cameraForward;
    float forward, strafe;

    void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        if (characterVisual == null)
        {
            characterVisual = transform;
        }
        camTransform = Camera.main.transform;
    }

    void Start()
    {
        characterController.detectCollisions = false;
        RecalculateCamera(Camera.main);
    }

    void Update()
    {
        mag = Mathf.Clamp01(new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical).sqrMagnitude);
        if (canStrafe)
        {
            lookToMovementDirection = false;
        }
        if (mag >= movementThreshold)
        {
            MovementAndRotation();
        }
        else
        {
            characterController.Move(new Vector3(0, verticalSpeed * Time.deltaTime, 0));
        }
        if (animator != null)
        {
            if (canStrafe)
            {
                RelativeAnimations();
            }
            else
            {
                animator.SetFloat(forwardAnimationVar, mag);
            }
        }

        if (isGrounded && jumpRequest)
        {
            verticalSpeed = Mathf.Sqrt(2 * jumpHeight * -gravity);
            isGrounded = false;
            jumpRequest = false;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }
    }

    void RelativeAnimations()
    {
        if (camTransform != null)
        {
            cameraForward = Vector3.Scale(camTransform.up, new Vector3(1, 0, 1)).normalized;
            move = moveJoystick.Vertical * cameraForward + moveJoystick.Horizontal * camTransform.right;
        }
        else
        {
            move = moveJoystick.Vertical * Vector3.forward + moveJoystick.Horizontal * Vector3.right;
        }
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        MoveAnims(move);
    }

    void MoveAnims(Vector3 move)
    {
        this.input = move;
        Vector3 localMove = transform.InverseTransformDirection(input);
        strafe = localMove.x;
        forward = localMove.z;
        animator.SetFloat(forwardAnimationVar, forward * 2f, 0.01f, Time.deltaTime);
        animator.SetFloat(strafeAnimationVar, strafe * 2f, 0.01f, Time.deltaTime);
    }

    void RecalculateCamera(Camera _cam)
    {
        Camera cam = _cam;
        camTransform = cam.transform;
        fwd = cam.transform.forward;
        fwd.y = 0;
        fwd = Vector3.Normalize(fwd);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * fwd;
    }

    void MovementAndRotation()
    {
        Vector3 direction = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
        Vector3 rightMovement = right * walkSpeed * Time.deltaTime * moveJoystick.Horizontal;
        Vector3 upMovement = fwd * walkSpeed * Time.deltaTime * moveJoystick.Vertical;
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        heading.y = verticalSpeed * Time.deltaTime;
        characterController.Move(heading * walkSpeed * Time.deltaTime);
        if (lookToMovementDirection)
        {
            characterVisual.forward = new Vector3(heading.x, characterVisual.forward.y, heading.z);
        }

        isGrounded = characterController.isGrounded;
    }

    public void OnJumpButtonPressed()
    {
        if (isGrounded)
        {
            jumpRequest = true;
        }
    }
}
