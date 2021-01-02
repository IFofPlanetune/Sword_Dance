using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimingManager : MonoBehaviour
{

    public int bpm = 140;
    public bool calibrationFlag;

    private float calCorr;
    private float maxTime;
    private List<float> calibrationDelays;

    private Dictionary<int, InputManager.attackType> pattern;
    
    private BeatWrapper beat;

    void Awake()
    {
        calibrationDelays = new List<float>();
        calibrationFlag = false;

        calCorr = 0;

        beat = new BeatWrapper();
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

    public bool BeatOne()
    {
        return beat.beatOne;
    }

    public bool BeatLast()
    {
        return beat.beatLast;
    }

    //enable the beat to sync up to audio sources
    public void Run()
    {
        float tps = bpm / 60f * (TimingParameters.smallestUnit / 4);
        maxTime = 1 / tps / 2;
        beat.bpm = bpm;
        beat.Run();
    }

    public void StopBeat()
    {
        beat.tSource.Cancel();
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
        float delay = maxTime - (curTime - beat.beatTime) - calCorr;
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
        id = beat.counter;
        if (delay < 0)
            id++;
        if (pattern.TryGetValue(id, out patAtk))
        {
            Debug.Log("index: " + id);
            Debug.Log("Type matching: " + (atk == patAtk));
            Debug.Log("Delay: " + delay / maxTime);
            return (Mathf.Abs(delay / maxTime) <= TimingParameters.threshold) && atk == patAtk;
        }
        return false;
    }

    public void SetDefense(Dictionary<int,InputManager.attackType> d)
    {
        pattern = d;
    }


    void OnDestroy()
    {
        StopBeat();
    }
}

public class BeatWrapper
{
    private TextMeshProUGUI status;

    public int bpm = 140;

    //public flags the activate on beginning of beat
    public bool beatOne;
    public bool beatLast;

    public float beatTime;
    public int counter;

    //token to cancel the beat
    public CancellationTokenSource tSource;

    public BeatWrapper()
    {
        tSource = new CancellationTokenSource();
        status = GameObject.FindGameObjectWithTag("Status").GetComponent<TextMeshProUGUI>();

        beatTime = 0;
        counter = 0;
    }

    //enable the beat to sync up to audio sources
    public void Run()
    {
        Beat();
    }

    //Coroutine for beat
    async void Beat()
    {
        float tps = (bpm / 60f) * (TimingParameters.smallestUnit / 4f);
        counter = 0;
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        float f = System.Diagnostics.Stopwatch.Frequency;
        float delay = 0;
        while (!tSource.Token.IsCancellationRequested)
        {
            timer.Restart();
            beatTime = Time.time;
            counter = (counter % TimingParameters.smallestUnit) + 1;
            if (counter == 1)
            {
                beatOne = true;
                beatLast = false;
            }
            else if (counter == TimingParameters.smallestUnit)
                beatLast = true;
            else
            {
                beatOne = false;
            }
            status.text = counter.ToString();
            await Task.Delay(TimeSpan.FromSeconds((1f / tps) - delay));
            timer.Stop();
            if(counter != 1)
                delay = (timer.ElapsedTicks / f) - ((1f / tps) - delay);
        }
    }
}
