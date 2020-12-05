using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public TimingManager TM;

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
    }

    public void Right(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        float delay = TM.CheckDelay();
        Debug.Log("Pressed Right with a delay of " + delay + "s");
    }
}
