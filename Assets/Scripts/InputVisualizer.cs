using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.Find("StartSlider").position;
        endPos = transform.Find("EndSlider").position;
        ball = transform.Find("Ball").gameObject;
    }

    public void Run()
    {
        duration = bpm / 60f;
        time = duration/8;
        StartCoroutine(Beat());
    }

    //Coroutine that moves the ball
    IEnumerator Beat()
    {
        while (true)
        {
            time += Time.deltaTime;
            time = Mathf.Min(time, duration);
            ball.transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            if (time == duration)
                time = 0;
            yield return null;
        }
    }

}
