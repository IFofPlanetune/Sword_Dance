using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameObject menu;
    private GameObject pointer;
    public GameManager GM;

    private bool active;
    private action selAction;
    private int pointerID;
    private int size;
    private Vector3 defaultPos;
    
    public enum action
    {
        attack,heal
    }

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Menu");
        pointer = menu.transform.GetChild(0).gameObject;

        active = true;
        selAction = action.attack;
        pointerID = 1;
        size = menu.transform.childCount - 1;
        defaultPos = pointer.transform.position;
    }

    public void TurnOff()
    {
        menu.SetActive(false);
        active = false;
    }

    public void TurnOn()
    {
        menu.SetActive(true);
        active = true;
    }

    public void Up()
    {
        if (!active)
            return;

        pointerID--;
        if (pointerID < 1)
            pointerID = size;
        float newY = ((pointerID - 1) * pointer.GetComponent<RectTransform>().rect.height);
        pointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(menu.transform.GetChild(pointerID).name);

    }

    public void Down()
    {
        Debug.Log("DOWN");
        if (!active)
            return;

        pointerID++;
        pointerID = ((pointerID - 1) % size) + 1;
        float newY = ((pointerID - 1) * pointer.GetComponent<RectTransform>().rect.height);
        pointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(menu.transform.GetChild(pointerID).name);
    }

    public void SwitchAction(string name)
    {
        switch(name)
        {
            case "Attack":
                selAction = action.attack;
                break;
            case "Heal":
                selAction = action.heal;
                break;
            default:
                break;
        }
    }

    public void Select()
    {
        if (!active)
            return;

        switch(selAction)
        {
            case action.attack:
                GM.Attack();
                break;
            case action.heal:
                GM.Heal();
                break;
            default:
                break;
        }
    }
}
