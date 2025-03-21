using System.Collections;
using System.Collections.Generic;
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
    private Vector3 endPosition;
    private Transform player;
    private bool isReturning = false;
    private bool isThrown = false;
    private bool isHolding = false;
    private PlayerInput playerInput;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        throwCappy.SetActive(false);
        cappy.SetActive(true);
        cappy2.SetActive(true);
    }

    void Update()
    {
        Debug.Log("isThrown: " + isThrown);
        Debug.Log("isReturning: " + isReturning);
        Debug.Log(endPosition);

        if (isThrown && !isReturning)
        {
            cappy2.SetActive(false);
            cappy.SetActive(false);
            throwCappy.SetActive(true);
            animatorCappy.SetTrigger("ja");

            if (Vector3.Distance(player.position, throwCappy.transform.position) >= maxDistance)
            {
                isReturning = true;
            }
            else
            {
                throwCappy.transform.position += startPosition * throwSpeed * Time.deltaTime;
            }
        }
        else if (isReturning)
        {
            StartCoroutine(WaitReturning());

            if (Vector3.Distance(throwCappy.transform.position, returnPoint.transform.position) < 0.1f)
            {
                isReturning = false;
                isThrown = false;
                throwCappy.transform.position = returnPoint.transform.position;
            }
        }
        else
        {
            cappy2.SetActive(true);
            cappy.SetActive(true);
            throwCappy.SetActive(false);
            throwCappy.transform.position = returnPoint.transform.position;
            throwCappy.transform.rotation = returnPoint.transform.rotation;
        }
    }

    public void OnThrowHat(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ThrowHat();
            OtherMovement.animator.SetTrigger("Throw");
        }

        if (context.performed)
        {
            isHolding = true;
            timer = 0f;
            StartCoroutine(HoldCappy());
        }

        if (context.canceled)
        {
            timer = 2f;
        }
    }

    private void ThrowHat()
    {
        if (!isThrown)
        {
            startPosition = player.forward;
            endPosition = player.forward * maxDistance;
            isThrown = true;
            isReturning = false;
            throwCappy.transform.position = returnPoint.transform.position;
            throwCappy.transform.forward = player.forward;
        }
    }

    private IEnumerator WaitReturning()
    {
        yield return new WaitForSeconds(.6f);
        throwCappy.transform.position = Vector3.MoveTowards(throwCappy.transform.position, returnPoint.transform.position, returnSpeed * Time.deltaTime);
    }

    private IEnumerator HoldCappy()
    {
        while (timer < 2)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        throwCappy.transform.position = Vector3.MoveTowards(throwCappy.transform.position, returnPoint.transform.position, returnSpeed * Time.deltaTime);
        isHolding = false;
    }
}