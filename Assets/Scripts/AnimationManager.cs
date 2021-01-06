using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator player;
    private Animator enemy;

    private GameObject slash;
    private GameObject explosion;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Animator>();
        slash = Resources.Load("Prefabs/Slash") as GameObject;
        explosion = Resources.Load("Prefabs/Explosion") as GameObject;
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

    public void PlayerAttack(InputManager.attackType type)
    {
        switch(type)
        {
            case InputManager.attackType.magic:
                PlayerExplosion();
                break;
            case InputManager.attackType.melee:
                PlayerSlash();
                break;
            default:
                break;
        }
    }

    void PlayerSlash()
    {
        GameObject slashInst = Instantiate(slash, 
            player.transform.position + new Vector3(2,0,0), slash.transform.rotation);
        Destroy(slashInst, 0.2f);
    }

    void PlayerExplosion()
    {
        GameObject expInst = Instantiate(explosion,
            player.transform.position + new Vector3(2, 0, 0), explosion.transform.rotation);
        Destroy(expInst, 0.2f);
    }
}
