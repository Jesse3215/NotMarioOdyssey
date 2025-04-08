using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameMangaer : MonoBehaviour
{
    public OtherMovement player;
    public Cappy Cappy;
    public EnemySlime slime;

    public CinemachineCamera playerCam;
    public CinemachineCamera slimeCam;

    public GameObject mario;
    public GameObject marioCappy;
    public GameObject slimeEnemy;
    public GameObject slimeEnemyCappy;

    private void Start()
    {
        EnablePlayer();

        playerCam.Priority = 100;
        slimeCam.Priority = 0;
    }

    public void EnablePlayer()
    {
        slimeEnemy.GetComponent<OtherMovement>().enabled = false;
        //slimeEnemy.GetComponent<PlayerInput>().Disable();
        mario.GetComponent<OtherMovement>().enabled = true;
        //mario.GetComponent<PlayerInput>().Enable();
        slimeCam.Priority = 0;
        playerCam.Priority = 100; 
    }

    public void EnableSlime()
    {
        mario.GetComponent<OtherMovement>().enabled = false;
        //mario.GetComponent<PlayerInput>().Disable();
        slimeEnemy.GetComponent<OtherMovement>().enabled = true;
        //slimeEnemy.GetComponent<PlayerInput>().Enable();
        playerCam.Priority = 0;
        slimeCam.Priority = 100;
    }

    private void Update()
    {
        if (mario.GetComponent<OtherMovement>().enabled == true)
        {
            slimeEnemyCappy.SetActive(false);
        }

        if (slime.GetComponent<OtherMovement>().enabled == true)
        {
            mario.SetActive(false);
            marioCappy.SetActive(false);
        }
    }
}
