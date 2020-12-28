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
                enemy.DeflectAttack(index);
            }
        }

        IV.Spawn(type);
    }

    public IEnumerator Attack()
    {
        MM.TurnOff();
        yield return new WaitUntil(() => TM.beatOne);
        isAttacking = true;
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Attack";
        yield return new WaitUntil(() => TM.beatFour);
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "";
        isAttacking = false;
        StartCoroutine(Defend());
    }


    public IEnumerator Heal()
    {
        //TO-DO
        Debug.Log("Heal selected");
        yield return new WaitForEndOfFrame();

        Defend();
    }

    public IEnumerator Defend()
    {
        TM.SetDefense(enemy.GetRandomPattern());
        //Wait empty beat for player to react
        yield return new WaitUntil(() => TM.beatOne);
        yield return new WaitUntil(() => TM.beatFour);

        yield return new WaitUntil(() => TM.beatOne);

        isDefending = true;
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Defend";

        yield return new WaitUntil(() => TM.beatFour);

        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "";
        isDefending = false;

        foreach(InputManager.attackType atk in enemy.SuccessfulAttacks())
        {
            Debug.Log(enemy.GetAttack(atk));
            Debug.Log(player.GetDefense(atk));
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
