using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator player;
    Animator enemy;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Animator>();
    }


    public void MovePlayerToEnemy()
    {
        player.SetInteger("Enemy", 1);
    }

    public void PlayerToIdle()
    {
        player.SetInteger("Enemy", 0);
    }

    public void EnemyToIdle()
    {
        enemy.SetBool("Attack", false);
    }

    public void MoveEnemyToPlayer()
    {
        enemy.SetBool("Attack", true);
    }
}
