﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public TimingManager TM;
    public GameManager GM;
    public MenuManager MM;

    public enum attackType
    {
        magic, melee
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Calibrate(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        if (!TM.calibrationFlag)
            TM.StartCalibration();
        else
            TM.EndCalibration();
    }

    public void Left(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        float delay = TM.CheckDelay();
        Debug.Log("Pressed Left with a delay of " + delay + "s");

        if(!TM.calibrationFlag)
            GM.HandleAction(delay, attackType.magic);
    }

    public void Right(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        float delay = TM.CheckDelay();
        Debug.Log("Pressed Right with a delay of " + delay + "s");
        if(!TM.calibrationFlag)
            GM.HandleAction(delay, attackType.melee);
    }

    public void MenuUp(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        MM.Up();
    }

    public void MenuDown(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        MM.Down();
    }

    public void MenuConfirm(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        MM.Select();
    }
}
