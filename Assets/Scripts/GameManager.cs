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
    public AnimationManager AnM;
    public AudioManager AuM;

    public Entity player;
    public Enemy enemy;

    public int bpm;
    private bool firstFrame;

    private TextMeshProUGUI status;

    public attackStyle atkStyle;
    private bool isAttacking;
    private bool isDefending;
    private bool defenseEnabled;
    private Pattern currentPattern;

    private string lastText;

    public enum attackStyle
    {
        free, pattern
    }

    void Start()
    {
        //set cross references
        MM.GM = this;
        MM.player = player;
        IM.GM = this;
        TM.GM = this;
        AuM.GM = this;
        IM.TM = TM;
        IM.MM = MM;
        AnM.AuM = AuM;
        

        player.GM = this;
        enemy.GM = this;

        isAttacking = false;
        isDefending = false;
        defenseEnabled = true;


        //start Timing Manager and Input Visualization
        TM.bpm = bpm;
        IV.bpm = bpm;
        TM.Run();
        IV.Run();

        //IV and TM are better synced when reset for some reason
        StartCoroutine(FixStartup());

        status = GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
    }

    IEnumerator FixStartup()
    {
        yield return new WaitUntil(() => TM.BeatLast());
        Reset();
        AuM.PlayBGM();
    }

    public void Reset()
    {
        TM.Reset();
        //Debug.Log("Reset");
        IV.Reset();
    }

    public void ChangeGranularity(int smallest)
    {
        TimingParameters.smallestUnit = smallest;
    }

    public void HandleAction(float delay, InputManager.attackType type)
    {
        if (!isAttacking && !isDefending && !TM.calibrationFlag)
            return;

        IV.Spawn(type);

        //Handle Attack
        if (isAttacking)
        {
            switch (atkStyle) {
                case attackStyle.pattern:
                    //TODO
                    break;
                case attackStyle.free:
                default:
                    if (TM.CheckAttack(delay))
                    {
                        Debug.Log("Successful Attack!");
                        AnM.PlayerAttack(type);
                        float dmg = Mathf.Max(player.GetAttack(type) - enemy.GetDefense(type), 1);
                        enemy.TakeDamage(dmg);
                    }
                    else
                    {
                        AnM.PlayerTrip();
                        IV.Disable();
                        isAttacking = false;
                    }
                    break;
            }
        }
        //Handle Defense
        if (isDefending)
        {
            if (!defenseEnabled)
                return;
            int index;
            AnM.PlayerDefense(type);
            if (TM.CheckDefense(delay, type, out index))
            {
                Debug.Log("Block successful!");
                currentPattern.MatchPattern(index);
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


    public void CalibrationOn()
    {
        TM.StartCalibration();
        status.text = "Calibrating";
        IV.Enable();
    }

    public void CalibrationOff()
    {
        TM.EndCalibration();
        status.text = "";
        IV.Disable();
    }

    public void EnemyAction(InputManager.attackType type)
    {
        AnM.EnemyAttack(type);
    }

    public IEnumerator FreeAttack()
    {
        MM.TurnOff();
        AnM.MovePlayerToEnemy();
        status.text = "Get Ready";

        //wait for new beat to start
        yield return new WaitUntil(() => TM.BeatLast());
        yield return new WaitUntil(() => TM.BeatOne());

        isAttacking = true;
        IV.Enable();
        status.text = "Attack";

        yield return new WaitUntil(() => TM.BeatLast());

        status.text = "Enemy Turn";
        isAttacking = false;
        IV.Disable();
        AnM.PlayerToIdle();
        StartCoroutine(Defend());
    }

    public IEnumerator PatternAttack(Pattern pattern)
    {
        //TODO
        yield return null;
    }


    public IEnumerator Heal()
    {
        MM.TurnOff();

        //if currently on Beat One, wait until roll-over
        yield return new WaitUntil(() => !TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatOne());

        status.text = "Healing";
        player.Heal(5);

        yield return new WaitUntil(() => TM.BeatLast());

        status.text = "Enemy Turn";
        StartCoroutine(Defend());
    }

    public IEnumerator Defend()
    {
        currentPattern = enemy.GetRandomPattern();
        currentPattern.Setup();
        TM.SetPattern(currentPattern.dictInst);
        //Wait empty beat for player to react
        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatLast());

        IV.Enable();
        isDefending = true;
        AnM.MoveEnemyToPlayer();
        status.text = "Defend";

        yield return new WaitUntil(() => TM.BeatOne());
        yield return new WaitUntil(() => TM.BeatLast());
        yield return new WaitUntil(() => TM.BeatOne());

        status.text = "";
        isDefending = false;
        IV.Disable();
        AnM.EnemyToIdle();

        float dmg = 0;
        foreach(InputManager.attackType atk in currentPattern.UnmatchedPatterns())
        {
            dmg += enemy.GetAttack(atk) - player.GetDefense(atk);
        }
        player.TakeDamage(dmg);

        currentPattern.DestroyInstance();
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
        status.text = "You Lose";
        player.Heal(100);
    }

    void Win()
    {
        status.text = "You Win";
        enemy.Heal(100);
    }
}
