using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TimingManager TM;
    public InputManager IM;
    public InputVisualizer IV;
    public AudioSource metronome;

    void Start()
    {
        //set cross references
        IM.GM = this;
        IM.TM = TM;

        //start Timing Manager and Audio source
        TM.Run();
        IV.Run();
        //metronome.Play();
    }

    void Update()
    {
        
    }

    public void UseMagic()
    {
        IV.Spawn(InputVisualizer.attackType.magic);
    }

    public void UseSword()
    {
        IV.Spawn(InputVisualizer.attackType.melee);
    }
}
