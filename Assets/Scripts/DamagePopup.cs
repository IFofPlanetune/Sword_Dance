using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro tmp;

    public float displayTime = 1f;
    public float speed = 2f;

    public static DamagePopup Create(int dmg, Transform parent)
    {
        DamagePopup instance = Instantiate(Resources.Load("Prefabs/DamagePopup") as GameObject, 
            parent.position + new Vector3(0,0.5f,0), Quaternion.identity).GetComponent<DamagePopup>();
        instance.tmp.text = dmg.ToString();
        return instance;
    }

    void Awake()
    {
        tmp = this.GetComponent<TextMeshPro>();
    }

    void Update()
    {
        transform.localPosition += new Vector3(0, speed, 0) * Time.deltaTime;

        displayTime -= Time.deltaTime;
        if (displayTime < 0)
            Destroy(this.gameObject);
    }
    
}
