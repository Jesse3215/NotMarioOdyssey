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
    Throwing,
}


public class OtherMovement : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    public Transform cameraTransform;
    private bool isGrounded = true;
    public Animator animator;

    [SerializeField] private Transform groundcheckTransform;
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
        moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        //isGrounded = Physics.Raycast(groundcheckTransform.position, Vector3.down, 0.2f, groundLayer);
        //Debug.DrawRay(groundcheckTransform.position, Vector3.down * 0.2f, Color.red);

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

        if (moveDirection.sqrMagnitude > 0.01f && isGrounded)
        {
            state = States.Walk;
        }

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (moveDirection != Vector3.zero)
        {
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        if (!isReadingInputs && isGrounded)
        {
            animator.SetBool("Walk", false);
            state = States.Idle;
        }

        if (!isReadingInputs)
        {
            m_rigidBody.velocity = Vector3.zero;
        }

        if (state != States.Jump && state != States.LongJump && isGrounded)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                state = isCrouching ? States.Crouch : States.Walk;
            }
        }

        switch (state)
        {
            case States.Idle:
                StartCoroutine(startIdleAnim());
                moveDirection = Vector3.zero;
                inputDirection = Vector3.zero;
                break;
            case States.Walk:
                moveSpeed = 5f;
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(moveDirection.x * moveSpeed, m_rigidBody.velocity.y, moveDirection.z * moveSpeed);
                animator.SetBool("Walk", true);
                break;
            case States.Jump:
                moveSpeed = 5f;
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);

                break;
            case States.LongJump:

                moveSpeed = 8f;

                animator.SetBool("Rolling", false);
                animator.Play("LongJump");

                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);

                if (isGrounded)
                {
                    state = isReadingInputs ? States.Walk : States.Idle;
                }

                break;

            case States.Crouch:
                moveSpeed = 2f;
                velocity = moveDirection * moveSpeed;
                m_rigidBody.velocity = new Vector3(velocity.x, m_rigidBody.velocity.y, velocity.z);
                if (isRolling)
                {
                    state = States.Rol;
                }
                break;
            case States.Rol:
                animator.SetBool("Crouch", false);
                animator.SetBool("Rolling", true);
                break;
            case States.Dive:
                if (!isGrounded)
                {
                    state = States.Dive;
                }
                if (isGrounded)
                {
                    state = isReadingInputs ? States.Walk : States.Idle;
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

    private IEnumerator startIdleAnim()
    {
        yield return new WaitForSeconds(1);
        animator.Play("Idle");
        StopCoroutine(startIdleAnim());
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
        GUI.Label(new Rect(10, 360, 360, 40), "isRolling: " + isRolling, m_Style);
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
        inputDirection = _context.ReadValue<Vector2>();
        isReadingInputs = _context.performed;

    }

    public void Jump(InputAction.CallbackContext _context)
    {
        if (_context.started && isGrounded)
        {
            if (isCrouching || isRolling)
            {
                PerformLongJump();
            }
            else
            {
                PerformRegularJump();
            }
        }
    }

    private void PerformRegularJump()
    {
        state = States.Jump;
        animator.SetTrigger("Jump");
        m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
        m_rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isGrounded = false;
    }

    private void PerformLongJump()
    {
        state = States.LongJump;
        animator.SetBool("Rolling", false);
        animator.SetBool("Crouch", false);
        animator.SetTrigger("LongJump");
        Vector3 jumpDirection = transform.forward * longJumpSpeed + Vector3.up * longJumpPower;
        m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
        m_rigidBody.AddForce(jumpDirection, ForceMode.Impulse);
        isGrounded = false;
        isCrouching = false;
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
            state = States.Rol;
            animator.SetBool("Crouch", false);
            animator.SetBool("Rolling", true);
            coroutine = StartCoroutine(RollCoroutine());
        }
        else if (_context.canceled)
        {
            isRolling = false;
            animator.SetBool("Rolling", false);
            moveSpeed = 5f;
            state = isReadingInputs ? States.Walk : States.Idle;
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
            state = isReadingInputs ? States.Walk : States.Idle;
        }
    }

}