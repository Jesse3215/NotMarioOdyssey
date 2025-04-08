using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySlime : MonoBehaviour
{
    public GameMangaer gamemanager;

    public GameObject hitTrigger;
    public GameObject hitTarget;

    public void OnEscape(InputAction.CallbackContext _context)
    {
        if(_context.performed)
        {
            EscapeEnemy();
        }
    }

    private void EscapeEnemy()
    {
        gamemanager.EnablePlayer();
    }
}
