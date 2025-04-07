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
    public GameObject slimeEnemy;

    private void Start()
    {
        playerCam.Priority = 100;
        slimeCam.Priority = 0;
    }

    public void EnablePlayer()
    {
        slimeEnemy.GetComponent<OtherMovement>().enabled = false;
        mario.GetComponent<OtherMovement>().enabled = true;
        slimeCam.Priority = 0;
        playerCam.Priority = 100; 
    }

    public void EnableSlime()
    {
        mario.GetComponent<OtherMovement>().enabled = false;
        slimeEnemy.GetComponent<OtherMovement>().enabled = true;
        playerCam.Priority = 0;
        slimeCam.Priority = 100;
    }
}
