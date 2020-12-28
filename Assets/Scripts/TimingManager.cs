using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimingManager : MonoBehaviour
{

    public int bpm = 140;
    public bool calibrationFlag;

    //public flags the activate on beginning and end of beat
    public bool beatOne;
    public bool beatFour;

    private float beatTime;
    private float maxTime;
    private float calCorr;
    private int counter;

    private Dictionary<int, InputManager.attackType> pattern;

    private List<float> calibrationDelays;

    //token to cancel the beat
    CancellationTokenSource tSource;

    void Awake()
    {
        calibrationDelays = new List<float>();
        calibrationFlag = false;

        beatOne = false;
        beatFour = false;

        beatTime = 0;
        maxTime = 0;
        calCorr = 0;
        counter = 0;

        tSource = new CancellationTokenSource();
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
        float tps = bpm / 60f * (TimingParameters.smallestUnit / 4);
        maxTime = 1 / tps / 2;
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

    public bool CheckAttack(float delay)
    {
        return Mathf.Abs(delay/maxTime) <= TimingParameters.threshold;
    }

    //checks if success is successful - id denotes which attack is being tried to defend against
    public bool CheckDefense(float delay, InputManager.attackType atk, out int id)
    {
        InputManager.attackType patAtk;
        id = counter;
        if (delay < 0)
            id++;
        if (pattern.TryGetValue(id, out patAtk))
            return (Mathf.Abs(delay / maxTime) <= TimingParameters.threshold) && atk == patAtk;
        return false;
    }

    public void SetDefense(Dictionary<int,InputManager.attackType> d)
    {
        pattern = d;
    }

    //Coroutine for beat
    async void Beat()
    {
        float tps = (bpm / 60f); //* (TimingParameters.smallestUnit / 4);
        counter = 0;
        while (!tSource.Token.IsCancellationRequested)
        {
            beatTime = Time.time;
            counter = (counter % 4) + 1;
            if(counter == 1)
            {
                beatOne = true;
                beatFour = false;
            }
            else if (counter == 4)
            {
                beatFour = true;
            }
            else
            {
                beatOne = false;
            }
            //print(counter);
            await Task.Delay(TimeSpan.FromSeconds(1f/tps));
        }
    }

    public void StopBeat()
    {
        tSource.Cancel();
    }

    void OnDestroy()
    {
        StopBeat();
    }
}
