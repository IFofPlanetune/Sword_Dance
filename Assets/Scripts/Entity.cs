using System.Collections;
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

    public void takeDamage(float dmg)
    {
        hp -= dmg;
        hpBar.value = hp / maxHP;
        hpText.text = (hp / maxHP * 100) + "%";
        if (hp <= 0)
            Defeat();
    }

    public void Defeat()
    {
        GM.HandleDeath(gameObject.tag);
    }
}
