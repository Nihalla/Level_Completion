using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{

    private NavMeshAgent agent;
    public GameObject final_destination;
    public List<GameObject> destination_list = new();
    public GameObject current_destination;
    public List<GameObject> held_items = new();
    public bool is_stuck = false;
    public bool in_loop = false;
    public bool out_of_reach = false;
    public bool inaccessible = false;
    public bool no_key = false;
    private float stuck_timer = 2f;
    private Vector2 previous_pos;
    private Vector2 current_pos;
    [SerializeField] private Material error;
    [SerializeField] private Material can_move;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destination_list.Add(final_destination);
        current_destination = final_destination;
        
        current_pos = new Vector2(transform.position.x, transform.position.y);
        previous_pos = current_pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        NavMeshPath path = new NavMeshPath();
        
        foreach (GameObject dest in destination_list)
        {
            if (dest == null)
            {
                RemoveDestination(dest);
            }
        }

        current_destination = destination_list[0];
            
        if (current_destination.tag == "Door")
        {
            agent.SetDestination(destination_list[0].transform.position);
        }
        else
        {
            if (agent.CalculatePath(destination_list[0].transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = can_move;
                //Debug.Log("Can reach");
                agent.SetDestination(destination_list[0].transform.position);
            }
            else
            {
                if(current_destination != final_destination)
                {
                    
                    if (destination_list[1].tag == "Door")
                    {
                        RemoveDestination(current_destination);
                        current_destination = destination_list[0];
                        Debug.Log("can't reach key, looking for alternatives");
                        current_destination.GetComponent<Door_logic>().KeyFailed();
                    }
                }
                gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = error;
                //Debug.Log("Can't reach?");
                out_of_reach = true;
            }
        }

        current_pos = new Vector2(transform.position.x, transform.position.y);

        if (Vector2.Distance(previous_pos, current_pos) < 0.02)
        {
            stuck_timer -= Time.deltaTime;
            if (stuck_timer <= 0)
            {
                if (current_destination != final_destination)
                {
                    if (destination_list[1].tag == "Door" && !no_key)
                    {
                        RemoveDestination(current_destination);
                        destination_list[0].GetComponent<Door_logic>().KeyFailed();
                        stuck_timer = 2f;
                    }
                    else if (destination_list[0] == destination_list[1] && !no_key && !inaccessible)
                    {
                        RemoveDestination(current_destination);
                        current_destination = destination_list[0];
                        RemoveDestination(current_destination);
                        if (destination_list[0].tag == "Door")
                        {
                            destination_list[0].GetComponent<Door_logic>().KeyFailed();
                            stuck_timer = 2f;
                        }
                    }
                    else
                    { is_stuck = true; }
                }
                else
                {
                    is_stuck = true;
                }
            }
        }
        else
        {
            stuck_timer = 2f;
            is_stuck = false;
        }

        previous_pos = current_pos;

        int list_pos = 0;
        foreach(GameObject destination in destination_list)
        {
            if (destination == current_destination)
            {
                list_pos++;
                if (list_pos > 1 && !inaccessible)
                {
                    //Debug.Log("Hi I am destination - " + destination.name + " and I am already on the list!");
                    is_stuck = true;
                    in_loop = true;
                }
            }
            /*if (destination_list.IndexOf(destination) != 0)
            {
                //Debug.Log(destination.name + " " + destination_list.IndexOf(destination));
            }
            //Debug.Log("Current dest " + current_destination.name);
            if (current_destination.name == destination.name && destination_list.IndexOf(destination) != 0)
            {
                Debug.Log("Hi I am destination - " + destination.name + " and I am already on the list with this index - " + destination_list.IndexOf(destination));
                
            }
            if (current_destination == destination && destination != destination_list[0])
            {
                //Debug.Log("Hi I am destination - " + destination.name + " and I am already on the list with this index - " + destination_list.IndexOf(destination));
                is_stuck = true;
            }*/
        }
        if (list_pos < 1)
        {
            is_stuck = false;
        }

        if (is_stuck)
        {
            gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = error;
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = can_move;
            in_loop = false;
            //out_of_reach = false;
            //no_key = false;
            //inaccessible = false;
        }
    }

    public void AddDestination(GameObject new_dest, int priority)
    {
       
        if (!in_loop)
        {
            Debug.Log("New Destination located, movign to " + new_dest.name);
            destination_list.Insert(priority, new_dest); }
    }

    public void RemoveDestination(GameObject to_remove)
    {
        Debug.Log("Removing destination - " + to_remove);
        if (destination_list.Contains(to_remove))
        {
            destination_list.Remove(to_remove);
        }
    }
    public void Hold(GameObject key)
    {
        held_items.Add(key);
    }
}
