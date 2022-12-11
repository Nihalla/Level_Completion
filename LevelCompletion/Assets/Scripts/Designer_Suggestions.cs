using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designer_Suggestions : MonoBehaviour
{
    private GameObject player;
    private AI_Movement player_script;
    private bool player_stuck;
    private bool player_looping;
    public bool show_suggestions = false;
    private ISSUE current_issue = ISSUE.NONE;
    private enum ISSUE
    {
        NOREACH = 0,
        LOOP = 1,
        NOKEY = 2,
        INACCESSIBLE = 3,
        NONE = 4,
        DEFAULT = -1
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_script = player.GetComponent<AI_Movement>();
        player_stuck = player_script.is_stuck;
        player_looping = player_script.in_loop;
    }

    // Update is called once per frame
    void Update()
    {
        player_stuck = player_script.is_stuck;
        player_looping = player_script.in_loop;

        if (player_stuck && !show_suggestions)
        {
            show_suggestions = true;
            current_issue = ISSUE.DEFAULT;
            if(player_looping)
            {
                current_issue = ISSUE.LOOP;
            }
            else if(player_script.out_of_reach)
            {
                current_issue = ISSUE.NOREACH;
            }
            else if(player_script.no_key)
            {
                current_issue = ISSUE.NOKEY;
            }
            else if(player_script.inaccessible)
            {
                current_issue = ISSUE.INACCESSIBLE;
            }
        }
        else
        {
            show_suggestions = false;
            current_issue = ISSUE.NONE;
        }
    }
}
