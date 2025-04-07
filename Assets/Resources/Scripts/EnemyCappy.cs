using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCappy : MonoBehaviour
{
    public Cappy Cappy;

    public GameObject slime;

    private void Start()
    {
        Cappy.cappyOnEnemy.SetActive(false);
    }

    private void Update()
    {
        Cappy.cappyOnEnemy.transform.position = slime.transform.position;
    }
}
