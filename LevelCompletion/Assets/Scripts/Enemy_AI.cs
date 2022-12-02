using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    [SerializeField] private GameObject move_point1;
    [SerializeField] private GameObject move_point2;
    private bool move_forward = true;
    //private float drop_rate = 100;
    private GameObject player;
    public GameObject loot;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float dist_to_point;
        if (move_forward)
        {
            agent.SetDestination(move_point2.transform.position);
            dist_to_point = Vector3.Distance(transform.position, move_point2.transform.position);
            if (dist_to_point <= 0.5f)
            {
                move_forward = !move_forward;
            }
        }
        else
        {
            agent.SetDestination(move_point1.transform.position);
            dist_to_point = Vector3.Distance(transform.position, move_point1.transform.position);
            if (dist_to_point <= 0.5f)
            {
                move_forward = !move_forward;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject == player)
        {
            player.GetComponent<AI_Movement>().Hold(loot);
            Debug.Log("Key picked up removing destination");
            player.gameObject.GetComponent<AI_Movement>().RemoveDestination(gameObject);
            gameObject.SetActive(false);
        }
    }
}
