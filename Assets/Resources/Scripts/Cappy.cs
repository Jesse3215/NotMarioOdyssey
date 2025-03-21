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
    public float maxDistance = 5f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Transform player;
    private bool isReturning = false;
    private bool isThrown = false;
    private PlayerInput playerInput;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animatorCappy = GetComponent<Animator>();

        

        throwCappy.SetActive(false);
        cappy.SetActive(true);
        cappy2.SetActive(true);
    }

    void Update()
    {
        Debug.Log("isThrown: " + isThrown);
        Debug.Log("isReturning: " + isReturning);

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
                //throwCappy.transform.position = Vector3.Lerp(startPosition, endPosition, Time.deltaTime);
            }
        }
        else if (isReturning)
        {
            throwCappy.transform.position = Vector3.MoveTowards(throwCappy.transform.position, returnPoint.transform.position, returnSpeed * Time.deltaTime);

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
        if (context.performed)
        {
            ThrowHat();
            OtherMovement.animator.SetTrigger("Throw");
        }
    }

    private void ThrowHat()
    {
        if (!isThrown)
        {
            startPosition = player.forward;
            startPosition.y += 0.5f;
            endPosition = player.forward * maxDistance;
            //endPosition.y += 0.5f;
            isThrown = true;
            isReturning = false;
            throwCappy.transform.position = returnPoint.transform.position;
            throwCappy.transform.forward = player.forward;
        }
    }
}