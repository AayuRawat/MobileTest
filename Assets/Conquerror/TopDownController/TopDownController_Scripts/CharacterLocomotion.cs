using UnityEngine;
using UnityEngine.UI;

public class CharacterLocomotion : MonoBehaviour
{
    [Tooltip("Assign animator if you would like. We are using 2d blendtree")]
    [SerializeField] Animator animator;
    [Tooltip("Character controller is a built in component in unity. Feel free to use rigidbody or changing transform directly")]
    [SerializeField] CharacterController characterController;
    [Tooltip("how fast the player walks")]
    [SerializeField] float walkSpeed = 3f;
    [Tooltip("if you would like separate visual from player assign something else here")]
    [SerializeField] Transform characterVisual;
    [Tooltip("Turn this off if you want to separate movement and aiming")]
    [SerializeField] bool lookToMovementDirection = true;
    [Tooltip("Feel free to assign other joysticks here")]
    [SerializeField] FixedJoystick moveJoystick;
    [Tooltip("Self explanatory. After this magnitude player will move ")]
    [SerializeField] float movementThreshold = 0.1f;
    [Header("Animation variables")]
    [Tooltip("This will turn rotation towards the joystick direction")]
    [SerializeField] bool canStrafe = false;
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string forwardAnimationVar = "Forward";
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string strafeAnimationVar = "Strafe";
    [Tooltip("Jump height")]
    [SerializeField] float jumpHeight = 2f;

    // Reference to the jump button
    [SerializeField] Button jumpButton;

    private float mag;
    private Transform camTransform;
    private Vector3 fwd, right;
    private Vector3 input, move;
    private Vector3 cameraForward;
    private float forward, strafe;
    private bool isGrounded = true;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private float verticalVelocity;

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

        // Assign the jump button listener
        jumpButton.onClick.AddListener(OnJumpButtonPressed);
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
            moveDirection = new Vector3(0, verticalVelocity, 0); // gravity when idle
            characterController.Move(moveDirection * Time.deltaTime);
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

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; // Small negative value to keep the player grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
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
        heading.y = verticalVelocity;
        moveDirection = heading * walkSpeed * Time.deltaTime;
        characterController.Move(moveDirection);

        if (lookToMovementDirection)
        {
            characterVisual.forward = new Vector3(heading.x, characterVisual.forward.y, heading.z);
        }
    }

    void OnJumpButtonPressed()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            UIManager.Instance.ShowLosePanel();
        }
    }
}
