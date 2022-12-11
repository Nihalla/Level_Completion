using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Designer_Suggestions : MonoBehaviour
{
    private GameObject player;
    private AI_Movement player_script;
    private bool player_stuck;
    private bool player_looping;
    public bool show_suggestions = false;
    public GameObject UI;
    public GameObject move_button;
    public GameObject remove_button;
    public GameObject change_key_button;
    public GameObject add_key_button;
    private GameObject[] doors;
    [SerializeField] private GameObject nearest_door;
    [SerializeField] private GameObject key_to_move;
    private GameObject[] walkable_tiles;
    [SerializeField]  private GameObject nearest_tile = null;
    private GameObject[] keys;
    [SerializeField] private GameObject unused_key;
    [SerializeField] private ISSUE current_issue = ISSUE.NONE;
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
        doors = GameObject.FindGameObjectsWithTag("Door");
        walkable_tiles = GameObject.FindGameObjectsWithTag("Floor_Tile");
        keys = GameObject.FindGameObjectsWithTag("Key");
    }

    // Update is called once per frame
    void Update()
    {

        player_stuck = player_script.is_stuck;
        player_looping = player_script.in_loop;

        if (player_stuck)
        {
            current_issue = ISSUE.DEFAULT;
            if (player_looping)
            {
                current_issue = ISSUE.LOOP;
            }
            else if (player_script.out_of_reach)
            {
                current_issue = ISSUE.NOREACH;
            }
            else if (player_script.no_key)
            {
                current_issue = ISSUE.NOKEY;
            }
            else if (player_script.inaccessible)
            {
                current_issue = ISSUE.INACCESSIBLE;
            }
            if (!show_suggestions)
            {
                show_suggestions = true;
                
            }
        }
        else
        {
            show_suggestions = false;
            current_issue = ISSUE.NONE;
        }

        UI.SetActive(show_suggestions);

        switch (current_issue)
        {
            case ISSUE.NOREACH:
                key_to_move = player_script.current_destination;
                float closest_distance = 999f;
                
                foreach (GameObject tile in walkable_tiles)
                {
                    if (tile != null)
                    {
                        //Debug.Log("Looking for closest door...");
                        if (Vector2.Distance(key_to_move.transform.position, tile.transform.position) < closest_distance)
                        {
                            closest_distance = Vector2.Distance(key_to_move.transform.position, tile.transform.position);
                            nearest_tile = tile;
                        }
                    }
                }

                break;
            case ISSUE.NOKEY:

                List<GameObject> used_keys = new();
                
                float closest_dist = 999f;
                foreach (GameObject door in doors)
                {

                    if (door != null)
                    {
                        //Debug.Log("Looking for closest door...");
                        if (Vector2.Distance(player.transform.position, door.transform.position) < closest_dist)
                        {
                            closest_dist = Vector2.Distance(player.transform.position, door.transform.position);
                            nearest_door = door;
                        }
                        
                        foreach(GameObject req_key in door.GetComponent<Door_logic>().required_key)
                        {
                            if(req_key != null && !used_keys.Contains(req_key))
                            {
                                used_keys.Add(req_key);
                            }
                        }
                        foreach (GameObject key in keys)
                        {
                            if (key != null)
                            {
                                if (used_keys != null)
                                {
                                    if (!used_keys.Contains(key))
                                    {
                                        unused_key = key;
                                    }
                                }
                            }
                        }
                        
                    }
                }
               
                break;
        }
    }

    public void AddKey()
    {
        nearest_door.GetComponent<Door_logic>().required_key.Add(unused_key);
        nearest_door.GetComponent<Door_logic>().ResolveDoor(0);
        player_script.no_key = false;
    }

    public void Move()
    {
        Debug.Log("Trying to move key");
        if(nearest_tile != null)
        {
            Debug.Log(key_to_move + " being moved to tile - " + nearest_tile);
            key_to_move.transform.position = new Vector3(nearest_tile.transform.position.x, nearest_tile.transform.position.y +1, nearest_tile.transform.position.z);
        }
    }
}
