using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_behavoiur : MonoBehaviour
{
    private bool is_active = false;
    public GameObject linked_portal;
    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            is_active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            is_active = false;
        }
    }

    public bool IsActive()
    {
        return is_active;
    }

    public void Teleport()
    {
        //Debug.Log("TELEPORT!");
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = new Vector3(linked_portal.transform.position.x, linked_portal.transform.position.y, player.transform.position.z);
        is_active = false;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
