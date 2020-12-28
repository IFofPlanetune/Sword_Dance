﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputVisualizer : MonoBehaviour
{

    public int bpm = 140;

    private Vector3 startPos;
    private Vector3 endPos;
    private GameObject ball;

    private float duration;
    private float time;

    private List<GameObject> attacks;
    
    private GameObject melee;
    private GameObject magic;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.Find("StartSlider").position;
        endPos = transform.Find("EndSlider").position;
        ball = transform.Find("Ball").gameObject;

        attacks = new List<GameObject>();
        melee = Resources.Load("Prefabs/Melee") as GameObject;
        magic = Resources.Load("Prefabs/Magic") as GameObject;
    }

    public void Run()
    {
        duration = 4 / (bpm / 60f * (TimingParameters.smallestUnit / 4));
        time = duration/8;
        StartCoroutine(Beat());
    }

    //Coroutine that moves the ball
    IEnumerator Beat()
    {
        while (true)
        {
            time += Time.deltaTime;
            ball.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Min(time / duration,duration));
            if (time > duration)
            {
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
