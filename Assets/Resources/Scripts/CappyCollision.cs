using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappyCollision : MonoBehaviour
{
    public Cappy Cappy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slime"))
        {
            Debug.Log("Hit Enemy");
            Cappy.GoToEnemy();
        }
    }
}
