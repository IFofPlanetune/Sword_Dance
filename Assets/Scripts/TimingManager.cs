using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimingManager : MonoBehaviour
{

    public int bpm = 140;
    public bool calibrationFlag;

    private float beatTime = 0;
    private float maxTime = 0;
    private float calCorr = 0;

    private List<float> calibrationDelays;

    void Awake()
    {
        calibrationDelays = new List<float>();
        calibrationFlag = false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (!calibrationFlag)
            return;
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }
        if(Keyboard.current.anyKey.wasPressedThisFrame)
        {
            float delay = CheckDelay();
            calibrationDelays.Add(delay);
            
        }
    }

    //enable the beat to sync up to audio sources
    public void Run()
    {
        float bps = bpm / 60f;
        maxTime = 1 / bps / 2;
        Beat();
    }

    //starts calibration process
    public void StartCalibration()
    {
        calibrationDelays = new List<float>();
        calibrationFlag = true;
        Debug.Log("Starting Calibration");
    }

    //ends calibration process and calculates delay correction
    public void EndCalibration()
    {
        calibrationFlag = false;

        if (calibrationDelays.Count == 0)
        {
            Debug.Log("Ending Calibration without any changes");
            return;
        }

        //take the median
        calibrationDelays.Sort();
        calCorr = calibrationDelays[calibrationDelays.Count/2];
        
        Debug.Log("Ending Calibration");
        Debug.Log("Avg delay is: " + calCorr);
    }

    //returns the delay between call of this function and beat
    public float CheckDelay()
    {
        float curTime = Time.time;
        float delay = maxTime - (curTime - beatTime) - calCorr;
        return delay;
    }

    //Coroutine for beat
    async void Beat()
    {
        float bps = bpm / 60f;
        int counter = 0;
        while (true)
        {
            beatTime = Time.time;
            counter = (counter % 4) + 1;
            //print(counter);
            await Task.Delay(TimeSpan.FromSeconds(1f/bps));
        }
    }
}
