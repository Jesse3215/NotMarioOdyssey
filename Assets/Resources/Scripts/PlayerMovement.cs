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
    private bool isGrounded = true;
    public Animator animator;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float groundPoundForce = 10f;
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float longJumpSpeed = 8f;
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
    private bool isReadingInputs = false;
    private bool isCrouching = false;

    private Vector3 velocity;

    private Coroutine coroutine;

    [SerializeField] private States state;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        originalTurnSmoothTime = turnSmoothTime;
    }
    private void Update()
    {
        Debug.Log("isGrounded: " + isGrounded);

        moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        //isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
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

        if (moveDirection.sqrMagnitude > 0.01f && isGrounded && state != States.Crouch && state != States.Rol)
        {
            state = States.Walk;
        }

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (moveDirection != Vector3.zero)
        {
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        if (!isReadingInputs && isGrounded && state != States.Jump)
        {
            animator.SetBool("Walk", false);
            state = States.Idle;
        }

        if(moveSpeed == 2)
        {
            state = States.Crouch;
        }

        switch (state)
        {
            case States.Idle:
                moveDirection = Vector3.zero;
                inputDirection = Vector3.zero;
                break;
            case States.Walk:
                moveSpeed = 5f;
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);
                animator.SetBool("Walk", true);
                break;
            case States.Jump:
                moveSpeed = 5f;
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);

                if (isGrounded)
                {
                    if (isReadingInputs)
                    {
                        state = States.Walk;
                    }
                    if (!isReadingInputs)
                    {
                        state = States.Idle;
                    }
                }

                break;
            case States.LongJump:

                moveSpeed = 8f;

                if (isGrounded)
                {
                    if (isReadingInputs)
                    {
                        state = States.Walk;
                    }
                    if (!isReadingInputs)
                    {
                        state = States.Idle;
                    }
                }

                break;

            case States.Crouch:
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);
                break;
            case States.Rol:
                animator.SetBool("Rolling", true);
                break;
            case States.Dive:
                if (isGrounded)
                {
                    if (isReadingInputs)
                    {
                        state = States.Walk;
                    }
                    if (!isReadingInputs)
                    {
                        state = States.Idle;
                    }
                }
                break;
            case States.GroundPound:

                if (isGrounded)
                {
                    animator.SetTrigger("Idle");
                    state = States.Idle;
                }

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
        GUI.Label(new Rect(10, 210, 360, 40), "State: " + state, m_Style);
        GUI.Label(new Rect(10, 260, 360, 40), "isGrounded: " + isGrounded, m_Style);
        GUI.Label(new Rect(10, 310, 360, 40), "isReadingInputs: " + isReadingInputs, m_Style);
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
        if(_context.performed)
        {
            isReadingInputs = true;
        }
        if (isReadingInputs)
        {
            inputDirection = _context.ReadValue<Vector2>();
        }
        if (_context.canceled)
        {
            isReadingInputs = false;
        }

    }

    public void Jump(InputAction.CallbackContext _context)
    {
        if (_context.started && isGrounded)
        {
            if (isCrouching)
            {
                state = States.LongJump;
                animator.SetBool("Crouch", false);
                animator.SetTrigger("LongJump");
                Vector3 jumpDirection = transform.forward * moveSpeed + Vector3.up * longJumpPower;
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
                m_rigidBody.AddForce(jumpDirection, ForceMode.Impulse);
                StartCoroutine(SetMoveSpeed());
            }
            else if (isRolling)
            {
                state = States.LongJump;
                animator.SetBool("Rolling", false);
                moveSpeed = 10f;
                animator.SetTrigger("LongJump");
                Vector3 jumpDirection = transform.forward * moveSpeed + Vector3.up * longJumpPower;
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
                m_rigidBody.AddForce(jumpDirection, ForceMode.Impulse);
                StartCoroutine(SetMoveSpeed());
            }
            else if (!isCrouching)
            {
                state = States.Jump;
                animator.SetTrigger("Jump");
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.y);
                m_rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                isGrounded = false;
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
            state = States.GroundPound;
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
            animator.SetBool("Crouch", false);
            state = States.Rol;
            coroutine = StartCoroutine(RollCoroutine());
        }
        else if (_context.canceled)
        {
            isRolling = false;
            state = States.Idle;
            moveSpeed = 5f;
        }
    }

    private IEnumerator RollCoroutine()
    {
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
        if (_context.performed && !isGrounded && state != States.Dive)
        {
            state = States.Dive;
            animator.SetTrigger("Dive");
            Vector3 diveDirection = transform.forward * diveForce + Vector3.up * diveForceUp;
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.AddForce(diveDirection, ForceMode.Impulse);
        }
    }

    public void Crouch(InputAction.CallbackContext _context)
    {
        if (_context.performed && isGrounded)
        {
            state = States.Crouch;
            isCrouching = true;
            moveSpeed = crouchSpeed;
            animator.SetBool("Crouch", true);
        }
        else if (_context.canceled)
        {
            animator.SetBool("Crouch", false);
            isCrouching = false;
            moveSpeed = 5f;
            if(isReadingInputs)
            {
                state = States.Walk;
            }
            if (!isReadingInputs)
            {
                state = States.Idle;
            }
        }
    }
}