using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private GameObject menu;
    private GameObject actionMenu;
    private GameObject attackMenu;
    private GameObject actionPointer;
    public GameManager GM;
    public Entity player;

    private bool active;

    public state currentState;
    private action selAction;
    private int actionPointerID;
    private int actionSize;
    private Vector3 defaultPos;

    private int attackID;
    private TextMeshProUGUI attackName;

    public enum action
    {
        attack, heal, pattern
    }

    public enum state
    {
        action, attack
    }

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Menu");
        actionMenu = menu.transform.GetChild(0).gameObject;
        attackMenu = menu.transform.GetChild(1).gameObject;

        actionPointer = actionMenu.transform.GetChild(0).gameObject;
        attackName = attackMenu.transform.Find("AttackName").GetComponent<TextMeshProUGUI>();

        active = true;
        StartActionMenu();
        actionSize = actionMenu.transform.childCount - 1;
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
        if (currentState != state.action)
            return;

        actionPointerID--;
        if (actionPointerID < 1)
            actionPointerID = actionSize;
        float newY = ((actionPointerID - 1) * actionPointer.GetComponent<RectTransform>().rect.height);
        actionPointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(actionMenu.transform.GetChild(actionPointerID).name);

    }

    public void Down()
    {
        if (!active)
            return;
        if (currentState != state.action)
            return;

        actionPointerID++;
        actionPointerID = ((actionPointerID - 1) % actionSize) + 1;
        float newY = ((actionPointerID - 1) * actionPointer.GetComponent<RectTransform>().rect.height);
        actionPointer.transform.position = defaultPos - new Vector3(0, newY);
        SwitchAction(actionMenu.transform.GetChild(actionPointerID).name);
    }

    public void SwitchAttack(int diff)
    {
        if (!active)
            return;
        if (currentState != state.attack)
            return;

        player.GetPattern(attackID).DestroyInstance();
        attackID = attackID + diff;
        if (attackID < 0)
            attackID += player.patterns.Count;
        attackID = attackID % player.patterns.Count;
        Pattern pattern = player.GetPattern(attackID);
        attackName.text = pattern.name;
        pattern.Setup();
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
        if (currentState == state.action)
        {
            switch (selAction)
            {
                case action.attack:
                    switch (GM.atkStyle)
                    {
                        case GameManager.attackStyle.pattern:
                            StartAttackMenu();
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
        else
        {
            //State = Attack
            StartActionMenu();
            StartCoroutine(GM.PatternAttack(player.GetPattern(attackID)));
        }
    }

    void StartActionMenu()
    {
        attackMenu.SetActive(false);
        actionMenu.SetActive(true);
        currentState = state.action;
        selAction = action.attack;
        actionPointerID = 1;
    }

    void StartAttackMenu()
    {
        actionMenu.SetActive(false);
        attackMenu.SetActive(true);
        currentState = state.attack;
        Pattern pattern = player.GetPattern(attackID);
        attackName.text = pattern.name;
        pattern.Setup();
    }
}
