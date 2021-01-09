using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TimingManager TM;
    public MenuManager MM;
    public InputManager IM;
    public InputVisualizer IV;
    public AnimationManager AM;

    public Entity player;
    public Enemy enemy;

    public int bpm;

    private bool isAttacking;
    private bool isDefending;

    void Start()
    {
        //set cross references
        MM.GM = this;
        IM.GM = this;
        IM.TM = TM;
        IM.MM = MM;

        player.GM = this;
        enemy.GM = this;

        isAttacking = false;
        isDefending = false;

        //start Timing Manager and Input Visualization
        TM.bpm = bpm;
        IV.bpm = bpm;
        TM.Run();
        IV.Run();
    }

    public void HandleAction(float delay, InputManager.attackType type)
    {
        if (!isAttacking && !isDefending)
            return;

        //Handle Attack
        if (isAttacking)
        {
            if (TM.CheckAttack(delay))
            {
                Debug.Log("Successful Attack!");
                AM.PlayerAttack(type);
                float dmg = Mathf.Max(player.GetAttack(type) - enemy.GetDefense(type), 1);
                enemy.TakeDamage(dmg);
            }
            else
            {
                AM.PlayerTrip();
                IV.Disable();
                isAttacking = false;
            }
        }
        //Handle Defense
        else
        {
            int index;
            if (TM.CheckDefense(delay,type, out index))
            {
                Debug.Log("Block successful!");
                enemy.DeflectAttack(index);
            }
        }

        IV.Spawn(type);
    }

    public IEnumerator Attack()
    {
        MM.TurnOff();
        AM.MovePlayerToEnemy();
        //wait for new beat to start
        yield return new WaitUntil(() => TM.BeatLast());
        yield return new WaitUntil(() => TM.BeatOne());

        isAttacking = true;
        IV.Enable();
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Attack";

        yield return new WaitUntil(() => TM.BeatLast());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Enemy Turn";
        isAttacking = false;
        IV.Disable();
        AM.PlayerToIdle();
        StartCoroutine(Defend());
    }


    public IEnumerator Heal()
    {
        MM.TurnOff();

        //if currently on Beat One, wait until roll-over
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Healing";
        player.Heal(5);

        yield return new WaitUntil(() => TM.BeatLast());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Enemy Turn";
        StartCoroutine(Defend());
    }

    public IEnumerator Defend()
    {
        TM.SetDefense(enemy.GetRandomPattern());
        //Wait empty beat for player to react
        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatLast());

        IV.Enable();
        isDefending = true;
        AM.MoveEnemyToPlayer();
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Defend";

        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatLast());
        yield return new WaitUntil(() => TM.BeatOne());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "";
        isDefending = false;
        IV.Disable();
        AM.EnemyToIdle();

        float dmg = 0;
        foreach(InputManager.attackType atk in enemy.SuccessfulAttacks())
        {
            dmg += enemy.GetAttack(atk) - player.GetDefense(atk);
        }
        player.TakeDamage(dmg);

        enemy.DestroyPattern();
        MM.TurnOn();
    }

    public void HandleDeath(string tag)
    {
        switch(tag)
        {
            case "Player":
                GameOver();
                break;
            case "Enemy":
                Win();
                break;
            default:
                break;
        }
    }

    void GameOver()
    {
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "You Lose";
    }

    void Win()
    {
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "You Win";
    }
}
