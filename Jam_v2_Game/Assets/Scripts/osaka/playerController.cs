using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    private NIS input;

    [Header("Salto")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpVelocity = 12f;
    [SerializeField] private float maxMaxHoldJumpTime = 0.35f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Aceleración del carro")]
    [SerializeField] private float baseAcceleration = 8f;
    [SerializeField] private float maxCarSpeed = 25f;

    public float CurrentSpeed { get; private set; }
    public float CurrentHoldJumpTime { get; private set; }
    public bool IsGrounded { get; private set; }

    private bool isJumping;
    private bool isJumpPressed; 
    private float jumpTimer;

    private void Awake()
    {
        input = new NIS();

        if (rb == null) rb = GetComponent<Rigidbody>();


        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        input.Enable();

        
        input.Player.Jump.started += OnJumpStarted;
        input.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        input.Player.Jump.started -= OnJumpStarted;
        input.Player.Jump.canceled -= OnJumpCanceled;

        input.Disable();
    }

    private void Update()
    {
       
        IsGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);

       
        if (IsGrounded && !WorldSpeedManager.Instance.IsRedLight)
        {
            float speedRatio = CurrentSpeed / maxCarSpeed;
            float currentAcceleration = baseAcceleration * (1f - speedRatio);

            CurrentSpeed = Mathf.Min(CurrentSpeed + currentAcceleration * Time.deltaTime, maxCarSpeed);
        }

        
        if (WorldSpeedManager.Instance.IsRedLight)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0f, Time.deltaTime * 3f);
        }

        
        WorldSpeedManager.Instance.SetWorldSpeed(CurrentSpeed);

        
        CurrentHoldJumpTime = maxMaxHoldJumpTime * (CurrentSpeed / maxCarSpeed);

       
        if (isJumpPressed && isJumping)
        {
            if (jumpTimer < CurrentHoldJumpTime)
            {
               
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
                jumpTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

 
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        isJumpPressed = true;

        if (IsGrounded)
        {
            isJumping = true;
            jumpTimer = 0f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
    }

  
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpPressed = false;
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
