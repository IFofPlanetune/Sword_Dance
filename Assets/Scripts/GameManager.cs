using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public TimingManager TM;
    public MenuManager MM;
    public InputManager IM;
    public InputVisualizer IV;
    public AnimationManager AM;

    public Entity player;
    public Enemy enemy;

    public AudioClip bgm;
    public int bpm;
    private AudioSource bgmSource;

    private bool isAttacking;
    private bool isDefending;
    private bool defenseEnabled;

    private string lastText;

    void Start()
    {
        //set cross references
        MM.GM = this;
        IM.GM = this;
        TM.GM = this;
        IM.TM = TM;
        IM.MM = MM;
        

        player.GM = this;
        enemy.GM = this;

        isAttacking = false;
        isDefending = false;
        defenseEnabled = true;

        bgmSource = this.GetComponent<AudioSource>();
        bgmSource.clip = bgm;

        //start Timing Manager and Input Visualization
        TM.bpm = bpm;
        IV.bpm = bpm;
        TM.Run();
        IV.Run();
        bgmSource.Play();
    }

    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log("Reset");
            TM.Reset();
            IV.Reset();
        }

    }

    public void HandleAction(float delay, InputManager.attackType type)
    {
        if (!isAttacking && !isDefending)
            return;

        IV.Spawn(type);

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
            if (!defenseEnabled)
                return;
            int index;
            AM.PlayerDefense(type);
            if (TM.CheckDefense(delay, type, out index))
            {
                Debug.Log("Block successful!");
                enemy.DeflectAttack(index);
            }
            else
            {
                StartCoroutine(DisableDefense());
            }
        }
    }

    IEnumerator DisableDefense()
    {
        defenseEnabled = false;
        IV.Disable();
        //wait for one tick
        yield return new WaitForSeconds(1 / (bpm / 60f * (TimingParameters.smallestUnit / 4)));
        defenseEnabled = true;
        IV.Enable();
    }

    public void EnemyAction(InputManager.attackType type)
    {
        AM.EnemyAttack(type);
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
