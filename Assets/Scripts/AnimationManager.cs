using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator player;
    private Animator enemy;

    private AudioSource sfxSource;

    private GameObject slash;
    private AudioClip slashSfx;
    private GameObject explosion;
    private AudioClip explosionSfx;
    private GameObject magicShield;
    private GameObject meleeShield;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Animator>();

        sfxSource = this.GetComponent<AudioSource>();

        slash = Resources.Load("Prefabs/Slash") as GameObject;
        slashSfx = Resources.Load("Sounds/Slash") as AudioClip;
        explosion = Resources.Load("Prefabs/Explosion") as GameObject;
        explosionSfx = Resources.Load("Sounds/Explosion") as AudioClip;
        magicShield = Resources.Load("Prefabs/MagicShield") as GameObject;
        meleeShield = Resources.Load("Prefabs/MeleeShield") as GameObject;
    }


    public void MovePlayerToEnemy()
    {
        player.SetInteger("Enemy", 1);
    }

    public void PlayerToIdle()
    {
        player.SetInteger("Enemy", 0);
        player.SetBool("Trip", false);
    }

    public void PlayerTrip()
    {
        player.SetBool("Trip", true);
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
        sfxSource.clip = slashSfx;
        sfxSource.Play();
        Destroy(slashInst, 0.2f);
    }

    void PlayerExplosion()
    {
        GameObject expInst = Instantiate(explosion,
            player.transform.position + new Vector3(2, 0, 0), explosion.transform.rotation);
        sfxSource.clip = explosionSfx;
        sfxSource.Play();
        Destroy(expInst, 0.2f);
    }

    public void PlayerDefense(InputManager.attackType type)
    {
        switch (type)
        {
            case InputManager.attackType.magic:
                PlayerMagicShield();
                break;
            case InputManager.attackType.melee:
                PlayerMeleeShield();
                break;
            default:
                break;
        }
    }

    void PlayerMagicShield()
    {
        GameObject maShieldInst = Instantiate(magicShield,
            player.transform.position, Quaternion.identity);
        Destroy(maShieldInst, 0.2f);
    }

    void PlayerMeleeShield()
    {
        GameObject meShieldInst = Instantiate(meleeShield,
           player.transform.position, Quaternion.identity);
        Destroy(meShieldInst, 0.2f);
    }

    public void EnemyAttack(InputManager.attackType type)
    {
        switch (type)
        {
            case InputManager.attackType.magic:
                EnemyExplosion();
                break;
            case InputManager.attackType.melee:
                EnemySlash();
                break;
            default:
                break;
        }
    }

    void EnemySlash()
    {
        GameObject slashInst = Instantiate(slash,
            player.transform.position, Quaternion.Euler(
                slash.transform.rotation.eulerAngles + new Vector3(0,0,180)));
        sfxSource.clip = slashSfx;
        sfxSource.Play();
        Destroy(slashInst, 0.2f);
    }

    void EnemyExplosion()
    {
        GameObject expInst = Instantiate(explosion,
            player.transform.position, explosion.transform.rotation);
        sfxSource.clip = explosionSfx;
        sfxSource.Play();
        Destroy(expInst, 0.2f);
    }


}
