using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    private bool selection_mode = false;
    private Vector3 selection_XY;
    private GameObject selected_key;
    [SerializeField] private Camera camera_main;

    PlayerControl controls;
    public enum ISSUE
    {
        NOREACH = 0,
        LOOP = 1,
        NOKEY = 2,
        INACCESSIBLE = 3,
        NONE = 4,
        DEFAULT = -1
    }
    private void Awake()
    {
        controls = new PlayerControl();
        controls.Designer.Select.performed += ctx => GetMouse();
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
            }
        }

        

        switch (current_issue)
        {
            case ISSUE.NOREACH:
                key_to_move = player_script.current_destination;
                float closest_distance = 999f;

                foreach (GameObject tile in walkable_tiles)
                {
                    if (tile != null)
                    {
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


                foreach (GameObject door in doors)
                {

                    if (door != null)
                    {

                        foreach (GameObject req_key in door.GetComponent<Door_logic>().required_key)
                        {
                            if (req_key != null && !used_keys.Contains(req_key))
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
        Debug.DrawRay(camera_main.transform.position, new Vector3(selection_XY.x, selection_XY.y, 0) - camera_main.transform.position, Color.red);
    }

    private void GetMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        selection_XY = Worldpos;
        if (selection_mode)
        {
            RaycastHit hit;

            if (Physics.Raycast(camera_main.transform.position, new Vector3(selection_XY.x, selection_XY.y, 0) - camera_main.transform.position, out hit))
            {
                Transform objectHit = hit.transform;

                if (objectHit.gameObject.tag == "Key")
                {
                    selected_key = objectHit.gameObject;
                }
                else if (selected_key != null && objectHit.gameObject.tag == "Floor_Tile")
                {
                    Debug.Log(selected_key + " being moved to tile - " + objectHit.gameObject);
                    selected_key.transform.position = new Vector3(objectHit.gameObject.transform.position.x, objectHit.gameObject.transform.position.y + 1, objectHit.gameObject.transform.position.z);
                    selected_key = null;
                    selection_mode = false;
                    camera_main.GetComponent<Camera>().orthographicSize = 5;
                }
            }
        }
    }

    public void AddKey()
    {
        bool in_inventory = false;
        if (unused_key != null)
        {
            foreach(GameObject item in player_script.held_items)
            {
                if(item!=null)
                {
                    if(unused_key == item)
                    {
                        in_inventory = true;
                    }
                }
            }
            if (in_inventory)
            {
                nearest_door.GetComponent<Door_logic>().required_key.Add(unused_key);
                nearest_door.GetComponent<Door_logic>().CheckIfNeededKeyInInventory();
                player_script.no_key = false;
            }
            else
            {
                nearest_door.GetComponent<Door_logic>().required_key.Add(unused_key);
                nearest_door.GetComponent<Door_logic>().ResolveDoor(0);
                player_script.no_key = false;
            }
        }
    }

    public void Move()
    {
        Debug.Log("Trying to move key");
        if(nearest_tile != null)
        {
            Debug.Log(key_to_move.name + " being moved to tile - " + nearest_tile.name);
            key_to_move.transform.position = new Vector3(nearest_tile.transform.position.x, nearest_tile.transform.position.y + 0.5f, nearest_tile.transform.position.z);
        }
    }

    public void Remove()
    {
        Debug.Log("Removing door...");
        {
            if(!player_looping)
            {
                Destroy(nearest_door);
            }
            else
            {
                Destroy(nearest_door);
            }
        }
    }

    public void Relocate()
    {
        selection_mode = !selection_mode;
        if(selection_mode)
        {
            camera_main.GetComponent<Camera>().orthographicSize = 8;
            Debug.Log("Select a key to move");
        }
        else
        {
            camera_main.GetComponent<Camera>().orthographicSize = 5;
            Debug.Log("Exited selection");
            selected_key = null;
        }
    }

    public ISSUE GetGiveIssue()
    {
        return current_issue;
    }

    private void OnEnable()
    {
        controls.Designer.Enable();
    }
    public void EnableInput()
    {
        controls.Designer.Enable();
    }
    private void OnDisable()
    {
        controls.Designer.Disable();
    }
    public void DisableInput()
    {
        controls.Designer.Disable();
    }
}
