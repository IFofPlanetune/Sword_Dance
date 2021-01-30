using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{

    public List<Sprite> tutorials;

    private int pointer;
    private Image sprite;

    // Start is called before the first frame update
    void Start()
    {
        pointer = 0;
        sprite = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            this.gameObject.SetActive(false);
        }
        if(Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            pointer = Mathf.Min(pointer+1, tutorials.Count - 1);
            sprite.sprite = tutorials[pointer];
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            pointer = Mathf.Max(pointer-1, 0);
            sprite.sprite = tutorials[pointer];
        }
    }
}
