using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TimingManager TM;
    public MenuManager MM;
    public InputManager IM;
    public InputVisualizer IV;

    void Start()
    {
        //set cross references
        MM.GM = this;
        IM.GM = this;
        IM.TM = TM;
        IM.MM = MM;

        //start Timing Manager and Audio source
        TM.Run();
        IV.Run();
    }

    public void UseMagic()
    {
        IV.Spawn(InputVisualizer.attackType.magic);
    }

    public void UseSword()
    {
        IV.Spawn(InputVisualizer.attackType.melee);
    }

    public void Attack()
    {
        //TO-DO
        Debug.Log("Attack selected");
    }

    public void Heal()
    {
        //TO-DO
        Debug.Log("Heal selected");
    }
}
