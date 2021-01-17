using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameManager GM;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private AudioClip slashSfx;
    private AudioClip explosionSfx;
    private AudioClip tripSfx;

    public AudioClip bgm;

    private float time;
    private bool bgmOn;

    void Awake()
    {
        bgmSource = this.GetComponents<AudioSource>()[0];
        sfxSource = this.GetComponents<AudioSource>()[1];
    }

    void Start()
    {
        explosionSfx = Resources.Load("Sounds/Explosion") as AudioClip;
        slashSfx = Resources.Load("Sounds/Slash") as AudioClip;
        tripSfx = Resources.Load("Sounds/Trip") as AudioClip;

        time = 0;
    }

    void Update()
    {
        if(bgmSource.isPlaying)
        {
            time += Time.deltaTime;
            if(time >= bgm.length)
            {
                GM.Reset();
                time = 0;
            }
        }
    }

    public void PlayBGM()
    {
        bgmSource.clip = bgm;
        bgmSource.Play();
    }

    public void AttackSfx(InputManager.attackType type)
    {
        AudioClip clip = null;
        switch(type)
        {
            case InputManager.attackType.magic:
                clip = explosionSfx;
                break;
            case InputManager.attackType.melee:
                clip = slashSfx;
                break;
            default:
                break;
        }
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void Trip()
    {
        sfxSource.clip = tripSfx;
        sfxSource.Play();
    }
}
