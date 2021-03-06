﻿using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public GameManager GM;
    public Slider hpBar;
    public TextMeshProUGUI hpText;

    public float maxHP;
    public float hp;
    public float atk;
    public float mAtk;
    public float def;
    public float mDef;

    public List<Pattern> patterns;
    private Dictionary<int, InputManager.attackType> dict;

    public float GetAttack(InputManager.attackType type)
    {
        switch(type)
        {
            case InputManager.attackType.magic:
                return mAtk;
            case InputManager.attackType.melee:
                return atk;
            default:
                return 0;
        }
    }

    public float GetDefense(InputManager.attackType type)
    {
        switch (type)
        {
            case InputManager.attackType.magic:
                return mDef;
            case InputManager.attackType.melee:
                return def;
            default:
                return 0;
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        DamagePopup.Create((int) dmg, transform);

        hpBar.value = hp / maxHP;
        hpText.text = (hp / maxHP * 100) + "%";

        if (hp <= 0)
            Defeat();
    }

    public void Heal(float h)
    {
        hp = Mathf.Min(hp+h,maxHP);
        hpBar.value = hp / maxHP;
        hpText.text = (hp / maxHP * 100) + "%";
    }

    public void Defeat()
    {
        GM.HandleDeath(gameObject.tag);
    }

    public Pattern GetPattern(int id)
    {
        return patterns[id];
    }

    public Pattern GetRandomPattern()
    {
        System.Random rand = new System.Random();
        int r = rand.Next(patterns.Count);
        return GetPattern(r);
    }

}
