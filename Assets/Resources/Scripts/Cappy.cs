using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cappy : MonoBehaviour
{
    public Animator animatorCappy;
    public OtherMovement OtherMovement;
    public GameObject throwCappy;
    public GameObject cappy;
    public GameObject cappy2;
    public GameObject returnPoint;
    public float throwSpeed = 10f;
    public float returnSpeed = 15f;
    public float maxDistance = 10f;
    private float timer;

    private Vector3 startPosition;
    private Transform player;
    private bool isReturning = false;
    private bool isThrown = false;
    private bool isHolding = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        throwCappy.SetActive(false);
        cappy.SetActive(true);
        cappy2.SetActive(true);
    }

    void Update()
    {
        if (isThrown && !isReturning)
        {
            cappy2.SetActive(false);
            cappy.SetActive(false);
            throwCappy.SetActive(true);
            animatorCappy.SetTrigger("ja");

            if (!isHolding)
            {
                throwCappy.transform.position += startPosition * throwSpeed * Time.deltaTime;
            }

            if (Vector3.Distance(player.position, throwCappy.transform.position) >= maxDistance && !isHolding)
            {
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            throwCappy.transform.position = Vector3.MoveTowards(throwCappy.transform.position, returnPoint.transform.position, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(throwCappy.transform.position, returnPoint.transform.position) < 0.1f)
            {
                ResetCappy();
            }
        }
        else
        {
            ResetCappy();
        }
    }

    public void OnThrowHat(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ThrowHat();
            OtherMovement.animator.SetTrigger("Throw");
        }
        else if (context.performed)
        {
            isHolding = true;
            timer = 0f;
            StartCoroutine(HoldingCappy());
        }
        else if (context.canceled)
        {
            isHolding = false;
            timer = 2f;
        }
    }

    private void ThrowHat()
    {
        if (!isThrown)
        {
            startPosition = player.forward;
            isThrown = true;
            isReturning = false;
            throwCappy.transform.position = returnPoint.transform.position;
            throwCappy.transform.forward = player.forward;
        }
    }

    private void ResetCappy()
    {
        isReturning = false;
        isThrown = false;
        throwCappy.SetActive(false);
        cappy.SetActive(true);
        cappy2.SetActive(true);
        throwCappy.transform.position = returnPoint.transform.position;
        throwCappy.transform.rotation = returnPoint.transform.rotation;
    }

    private IEnumerator HoldingCappy()
    {
        while(timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isReturning = true;
    }
}
