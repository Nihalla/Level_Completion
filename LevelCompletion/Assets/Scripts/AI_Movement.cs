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
    [SerializeField] private Material error;
    [SerializeField] private Material can_move;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destination_list.Add(final_destination);
        current_destination = final_destination;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        NavMeshPath path = new NavMeshPath();
        
        
        if (agent.CalculatePath(destination_list[0].transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = can_move;
            //Debug.Log("Can reach");
            agent.SetDestination(destination_list[0].transform.position);
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = error;
            //Debug.Log("Can't reach?");
        }
        //agent.SetDestination(destination_list[0].transform.position);

    }

    public void AddDestination(GameObject new_dest)
    {
        Debug.Log("Key located, movign to key");
        destination_list.Insert(0, new_dest);
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
