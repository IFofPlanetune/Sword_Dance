using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputVisualizer : MonoBehaviour
{

    public int bpm = 140;

    private Vector3 startPos;
    private Vector3 endPos;
    private GameObject ball;
    private Vector3 ballStart;

    private float duration;
    private float time;
    private Coroutine runningBeat;

    private List<GameObject> attacks;
    
    private GameObject melee;
    private GameObject magic;

    private bool active;
    private CanvasGroup cg;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.Find("StartSlider").position;
        endPos = transform.Find("EndSlider").position;
        ball = transform.Find("Ball").gameObject;
        ballStart = transform.Find("BallStart").position;

        attacks = new List<GameObject>();
        melee = Resources.Load("Prefabs/Melee") as GameObject;
        magic = Resources.Load("Prefabs/Magic") as GameObject;

        active = true;
        cg = GetComponent<CanvasGroup>();
    }

    public void Disable()
    {
        active = false;
        cg.alpha = 0.4f;
    }

    public void Enable()
    {
        active = true;
        cg.alpha = 1f;
    }

    public void Run()
    {
        duration = 4 / (bpm / 60f);
        time = duration / 4;
        runningBeat = StartCoroutine(Beat());
    }

    public void Reset()
    {
        StopCoroutine(runningBeat);
        ball.transform.position = ballStart;
        time = duration / 16;
        runningBeat = StartCoroutine(Beat());
    }

    //Coroutine that moves the ball
    IEnumerator Beat()
    {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        float f = System.Diagnostics.Stopwatch.Frequency;
        timer.Restart();
        while (true)
        {
            time += Time.deltaTime;
            ball.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Min(time / duration, 1));
            if (time >= duration)
            {
                timer.Stop();
                //Debug.Log("IV: " + timer.ElapsedMilliseconds);
                timer.Restart();
                foreach (GameObject g in attacks)
                    Destroy(g);
                attacks.Clear();
                time -= duration;
            }
            yield return null;
        }
    }

    public void Spawn(InputManager.attackType type)
    {
        if (!active)
            return;

        GameObject atk = null;
        switch(type)
        {
            case InputManager.attackType.magic:
                atk = magic;
                break;
            case InputManager.attackType.melee:
                atk = melee;
                break;
            default:
                break;
        }
        attacks.Add(Instantiate(atk, ball.transform.position, Quaternion.identity, transform));
    }

}
