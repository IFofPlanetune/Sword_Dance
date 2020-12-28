using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public List<GameObject> patterns;
    private GameObject currentPattern;
    private Dictionary<int, InputManager.attackType> dict;

    public Dictionary<int,InputManager.attackType> GetPattern(int id)
    {
        dict = new Dictionary<int,InputManager.attackType>();

        currentPattern = Instantiate(patterns[id], GameObject.Find("Canvas").transform);

        //Create dict from name
        string name = currentPattern.name.Split('(')[0];
        
        List<int> numbers = new List<int>();
        foreach(string s in name.Split('-'))
        {
            try
            {
                int number = Int32.Parse(s);
                numbers.Add(number);
            }
            catch 
            {
                InputManager.attackType type;
                switch(s)
                {
                    case "Blue":
                        type = InputManager.attackType.melee;
                        break;
                    case "Red":
                        type = InputManager.attackType.magic;
                        break;
                    default:
                        type = InputManager.attackType.melee;
                        break;
                }
                foreach(int i in numbers)
                {
                    dict.Add(i, type);
                }
                numbers.Clear();
            }
        }

        return dict;
    }

    public Dictionary<int, InputManager.attackType> GetRandomPattern()
    {
        System.Random rand = new System.Random();
        int r = rand.Next(patterns.Count);
        return GetPattern(r);
    }

    public void DestroyPattern()
    {
        Destroy(currentPattern);
    }

    public void DeflectAttack(int id)
    {
        dict.Remove(id);
    }

    //returns a list of all non-deflected attacks
    public List<InputManager.attackType> SuccessfulAttacks()
    {
        return dict.Values.ToList();
    }
}
