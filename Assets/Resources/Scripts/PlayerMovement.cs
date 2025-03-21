using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum States 
{ 
    Idle,
    Walk,
    Jump,
    LongJump,
    Crouch,
    Rol,
    Dive,
    GroundPound,
}


public class OtherMovement : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    public Transform cameraTransform;
    private bool isGrounded;
    public Animator animator;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float groundPoundForce = 10f;
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float longJumpPower;
    [SerializeField] private float diveForce = 7f;
    [SerializeField] private float diveForceUp = 10f;

    private float angle;
    private float targetAngle;
    private float lastAngle;
    //private int jumpCount = 0;
    private Vector3 moveDirection;
    private bool isRolling = false;
    private Vector3 rollDirection;
    private Vector2 inputDirection;
    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.05f;
    private float originalTurnSmoothTime;
    private bool isReadingInputs = true;
    private bool isCrouching = false;

    private Vector3 velocity;

    private Coroutine coroutine;

    private States states;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        originalTurnSmoothTime = turnSmoothTime;
    }
    private void Update()
    {
        Debug.Log("isGrounded: " + isGrounded);

        moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.red);

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            lastAngle = angle;
        }
        else
        {
            moveDirection = Vector3.zero;
            angle = lastAngle;
        }

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (moveDirection != Vector3.zero)
        {
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        velocity = moveDirection * moveSpeed;
        m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);

        switch (states)
        {
            case States.Idle:
                
                break;
            case States.Walk:

                break;
            case States.Jump:

                break;
            case States.LongJump:

                break;
            case States.Crouch:

                break;
            case States.Rol:

                break;
            case States.Dive:

                break;
            case States.GroundPound:

                break;
        }
    }

    private void OnGUI()
    {
        if (!enableDebug) return;

        GUIStyle m_Style = new GUIStyle
        {
            fontSize = 24,
            normal = { textColor = Color.white }
        };

        GUI.Label(new Rect(10, 10, 300, 40), "Angle: " + angle, m_Style);
        GUI.Label(new Rect(10, 60, 300, 40), "Target Angle: " + targetAngle, m_Style);
        GUI.Label(new Rect(10, 110, 360, 40), "Move Direction: " + moveDirection, m_Style);
        GUI.Label(new Rect(10, 160, 360, 40), "Input Direction: " + inputDirection, m_Style);
    }

        private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    public void OnMove(InputAction.CallbackContext _context)
    {
        if (isReadingInputs)
        {
            animator.SetTrigger("Walk");
            inputDirection = _context.ReadValue<Vector2>();
        }

    }

    public void Jump(InputAction.CallbackContext _context)
    {
        if (_context.started && isGrounded)
        {
            if (isCrouching)
            {
                animator.SetBool("Crouch", false);
                moveSpeed = 10f;
                animator.SetTrigger("LongJump");
                Vector3 jumpDirection = transform.forward * moveSpeed + Vector3.up * longJumpPower;
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
                m_rigidBody.AddForce(jumpDirection, ForceMode.Impulse);
                StartCoroutine(SetMoveSpeed());
            }
            else if (!isCrouching)
            {
                animator.SetTrigger("Jump");
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.y);
                m_rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator SetMoveSpeed()
    {
        if (isGrounded)
        {
            moveSpeed = 5f;
            isCrouching = false;
            yield return null;
        }
    }

    public void GroundPound(InputAction.CallbackContext _context)
    {
        if (_context.performed && !isGrounded)
        {
            m_rigidBody.velocity = Vector3.zero;
            inputDirection = Vector3.zero;
            animator.SetTrigger("Groundpound");
            StartCoroutine(SetGravity());

        }
    }

    private IEnumerator SetGravity()
    {
        isReadingInputs = false;
        m_rigidBody.useGravity = false;
        yield return new WaitForSeconds(.4f);
        m_rigidBody.useGravity = true;
        m_rigidBody.AddForce(Vector3.down * groundPoundForce, ForceMode.Impulse);
        isReadingInputs = true;
    }

    public void Rolling(InputAction.CallbackContext _context)
    {
        if (_context.performed && isGrounded && !isRolling)
        {
            isRolling = true;
            coroutine = StartCoroutine(RollCoroutine());
        }
        else if (_context.canceled)
        {
            isRolling = false;
            moveSpeed = 5f;
        }
    }

    private IEnumerator RollCoroutine()
    {
        animator.SetBool("Rolling", true);

        turnSmoothTime = turnSmoothTime * 10;

        while (isRolling && isGrounded)
        {
            moveSpeed = 7.5f;

            rollDirection = transform.forward;

            m_rigidBody.velocity = rollDirection * rollSpeed;

            yield return new WaitForEndOfFrame();
        }

        m_rigidBody.velocity = Vector3.zero;
        animator.SetBool("Rolling", false);
        turnSmoothTime = originalTurnSmoothTime;
        StopCoroutine(coroutine);
    }

    public void Dive(InputAction.CallbackContext _context)
    {
        if (_context.performed && !isGrounded)
        {
            animator.SetTrigger("Dive");
            Vector3 diveDirection = transform.forward * diveForce + Vector3.up * diveForceUp;
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.AddForce(diveDirection, ForceMode.Impulse);
            StartCoroutine(ResetDive());
        }
    }

    private IEnumerator ResetDive()
    {
        isReadingInputs = false;
        yield return new WaitForSeconds(1.2f);
        isReadingInputs = true;
    }

    public void Crouch(InputAction.CallbackContext _context)
    {
        if (_context.performed && isGrounded)
        {
            isCrouching = true;
            moveSpeed = crouchSpeed;
            animator.SetBool("Crouch", true);
        }
        else if (_context.canceled)
        {
            isCrouching = false;
            moveSpeed = 5f;
            animator.SetBool("Crouch", false);
        }
    }
}