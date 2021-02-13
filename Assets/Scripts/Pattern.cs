using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Pattern : ScriptableObject
{
    public List<int> keys;
    public List<InputManager.attackType> values;
    public GameObject visualization;
    private GameObject vInstance;
    public Dictionary<int, InputManager.attackType> dictInst;

    public void Setup()
    {
        SpawnInstance();
        CreateDict();
    }

    private Dictionary<int, InputManager.attackType> CreateDict()
    {
        Dictionary<int, InputManager.attackType> dict = new Dictionary<int, InputManager.attackType>();
        for (int i = 0; i < keys.Count; i++)
        {
            dict.Add(keys[i], values[i]);
        }
        dictInst = dict;
        return dict;
    }

    private void SpawnInstance()
    {
        vInstance = Instantiate(visualization, GameObject.Find("InputVisualizer").transform);
    }

    public void DestroyInstance()
    {
        Destroy(vInstance);
    }

    public void MatchPattern(int id)
    {
        dictInst.Remove(id);
    }

    //returns a list of all non-deflected attacks
    public List<InputManager.attackType> UnmatchedPatterns()
    {
        return dictInst.Values.ToList();
    }

    public void CreateValues()
    {
        keys = new List<int>();
        values = new List<InputManager.attackType>();

        string name = visualization.name.Split('(')[0];

        List<int> numbers = new List<int>();
        foreach (string s in name.Split('-'))
        {
            try
            {
                int number = Int32.Parse(s);
                numbers.Add(number);
            }
            catch
            {
                InputManager.attackType type;
                switch (s)
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
                foreach (int i in numbers)
                {
                    keys.Add(i);
                    values.Add(type);
                }
                numbers.Clear();
            }
        }
    }

}