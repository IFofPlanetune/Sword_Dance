using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TimingManager TM;
    public InputManager IM;
    public AudioSource metronome;

    void Start()
    {
        //start Timing Manager and Audio source
        TM.StartTimer();
        metronome.Play();

        //set reference in InputManager
        IM.TM = TM;
    }

    void Update()
    {
        
    }
}
