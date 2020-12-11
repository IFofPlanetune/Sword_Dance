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
        //start Timing Manager and Audio source
        TM.Run();
        IV.Run();
        metronome.Play();

        //set reference in InputManager
        IM.TM = TM;
    }

    void Update()
    {
        
    }
}
