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
    public Entity enemy;

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

        //start Timing Manager and Audio source
        TM.Run();
        IV.Run();
    }

    public void UseMagic()
    {
        if (!isAttacking && !isDefending)
            return;

        //Handle Attack
        if (isAttacking)
        {
            float dmg = Mathf.Max(player.mAtk - enemy.mDef, 1);
            enemy.takeDamage(dmg);
        }
        //Handle Defense
        else
        {

        }

        IV.Spawn(InputVisualizer.attackType.magic);
    }

    public void UseSword()
    {
        if (!isAttacking && !isDefending)
            return;

        //Handle Attack
        if(isAttacking)
        {
            float dmg = Mathf.Max(player.atk - enemy.def,1);
            enemy.takeDamage(dmg);
        }
        //Handle Defense
        else
        {

        }

        IV.Spawn(InputVisualizer.attackType.melee);
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
        //Wait empty beat for player to react
        yield return new WaitUntil(() => TM.beatOne);
        yield return new WaitUntil(() => TM.beatFour);
        yield return new WaitUntil(() => TM.beatOne);
        isDefending = true;
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "Defend";
        yield return new WaitUntil(() => TM.beatFour);
        GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>().text = "";
        isDefending = false;
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
