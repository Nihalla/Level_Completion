using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Text_Script : MonoBehaviour
{
    [SerializeField] private Designer_Suggestions issues;

    void FixedUpdate()
    {
        switch (issues.GetGiveIssue())
        {
            case Designer_Suggestions.ISSUE.NOREACH:
                GetComponent<TMP_Text>().text = "Key out of bounds";
                break;
            case Designer_Suggestions.ISSUE.NOKEY:
                GetComponent<TMP_Text>().text = "No key attached to the door";
                break;
            case Designer_Suggestions.ISSUE.LOOP:
                GetComponent<TMP_Text>().text = "Stuck in a loop";
                break;
            case Designer_Suggestions.ISSUE.INACCESSIBLE:
                GetComponent<TMP_Text>().text = "Key behind something and can not be reached";
                break;
            case Designer_Suggestions.ISSUE.DEFAULT:
                GetComponent<TMP_Text>().text = "Unknown issue";
                break;
            case Designer_Suggestions.ISSUE.NONE:
                GetComponent<TMP_Text>().text = "Moving";
                break;
        }

    }
}
