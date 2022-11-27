using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_logic : MonoBehaviour
{
    //private bool open = false;
    public GameObject required_key;
    private GameObject player;
    private bool unlocked = false;
    // Start is called before the first frame update

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public bool CheckKey(GameObject key)
    {
        if (key == required_key)
        {
            //open = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            foreach (GameObject item in player.GetComponent<AI_Movement>().held_items)
            {
                if (item != null)
                {
                    if (CheckKey(item))
                    {
                        player.GetComponent<AI_Movement>().held_items.Remove(item);
                        Destroy(item);
                        unlocked = true;
                        Destroy(gameObject);
                        break;
                    }
                }
            }

            if (!unlocked)
            {
                Debug.Log("Player hit a door , locating key");
                player.GetComponent<AI_Movement>().AddDestination(required_key);
            }
        }
    }
}
