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
                float dmg = Mathf.Max(player.GetAttack(type) - enemy.GetDefense(type), 1);
                enemy.TakeDamage(dmg);
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
        //if currently on Beat One, wait until roll-over
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());
        isAttacking = true;
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Attack";
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Enemy Turn";
        isAttacking = false;
        StartCoroutine(Defend());
    }


    public IEnumerator Heal()
    {
        //TO-DO
        Debug.Log("Heal selected");
        MM.TurnOff();
        //if currently on Beat One, wait until roll-over
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Healing";
        player.Heal(5);

        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());

        StartCoroutine(Defend());
    }

    public IEnumerator Defend()
    {
        TM.SetDefense(enemy.GetRandomPattern());
        //Wait empty beat for player to react
        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => !TM.BeatOne());

        isDefending = true;
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Defend";

        yield return new WaitUntil(() => TM.BeatOne());

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "";
        isDefending = false;

        foreach(InputManager.attackType atk in enemy.SuccessfulAttacks())
        {
            float dmg = enemy.GetAttack(atk) - player.GetDefense(atk);
            player.TakeDamage(dmg);
        }
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
