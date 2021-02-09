using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameObject menu;
    private GameObject actionMenu;
    private GameObject actionPointer;
    public GameManager GM;

    private bool active;
    private action selAction;
    private int actionPointerID;
    private int size;
    private Vector3 defaultPos;
    
    public enum action
    {
        attack,heal,pattern
    }

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Menu");
        actionMenu = menu.transform.GetChild(0).gameObject;
        actionPointer = actionMenu.transform.GetChild(0).gameObject;

        active = true;
        selAction = action.attack;
        actionPointerID = 1;
        size = actionMenu.transform.childCount - 1;
        defaultPos = actionPointer.transform.position;
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

        actionPointerID--;
        if (actionPointerID < 1)
            actionPointerID = size;
        float newY = ((actionPointerID - 1) * actionPointer.GetComponent<RectTransform>().rect.height);
        actionPointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(actionMenu.transform.GetChild(actionPointerID).name);

    }

    public void Down()
    {
        if (!active)
            return;

        actionPointerID++;
        actionPointerID = ((actionPointerID - 1) % size) + 1;
        float newY = ((actionPointerID - 1) * actionPointer.GetComponent<RectTransform>().rect.height);
        actionPointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(actionMenu.transform.GetChild(actionPointerID).name);
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
                switch (GM.atkStyle)
                {
                    case GameManager.attackStyle.pattern:
                        StartCoroutine(GM.PatternAttack(null));
                        break;
                    case GameManager.attackStyle.free:
                    default:
                        StartCoroutine(GM.FreeAttack());
                        break;
                }
                break;
            case action.heal:
                StartCoroutine(GM.Heal());
                break;
            default:
                break;
        }
    }
}
