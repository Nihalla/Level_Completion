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
    private bool is_stuck = false;
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
                    RemoveDestination(current_destination);
                    current_destination = destination_list[0];
                    if (current_destination.tag == "Door")
                    {
                        Debug.Log("can't reach key, looking for alternatives");
                        current_destination.GetComponent<Door_logic>().KeyFailed();
                    }
                }
                gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = error;
                //Debug.Log("Can't reach?");
            }
        }
        current_pos = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(previous_pos, current_pos) < 0.01)
        {
            stuck_timer -= Time.deltaTime;
            if (stuck_timer <= 0)
            {
                is_stuck = true; 
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
                if (list_pos > 1)
                {
                    //Debug.Log("Hi I am destination - " + destination.name + " and I am already on the list!");
                    is_stuck = true;
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
        }
    }

    public void AddDestination(GameObject new_dest, int priority)
    {
        //Debug.Log("New Destination located, movign to " + new_dest.name);
        destination_list.Insert(priority, new_dest);
    }

    public void RemoveDestination(GameObject to_remove)
    {
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
