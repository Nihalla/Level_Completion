using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Text_Script : MonoBehaviour
{
    private bool stuck = false;
    // Start is called before the first frame update
    void Start()
    {
        stuck = GameObject.FindGameObjectWithTag("Player").GetComponent<AI_Movement>().is_stuck;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stuck = GameObject.FindGameObjectWithTag("Player").GetComponent<AI_Movement>().is_stuck;
        if (stuck)
        {
            GetComponent<TMP_Text>().text = "Stuck";
        }
        else
        {
            GetComponent<TMP_Text>().text = "Moving";
        }
    }
}
