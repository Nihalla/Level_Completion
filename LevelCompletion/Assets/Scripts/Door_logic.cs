using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_logic : MonoBehaviour
{
    //private bool open = false;
    public List<GameObject> required_key = new();
    private GameObject player;
    private bool unlocked = false;
    private int keys_failed = 0;
    private Enemy_Manager enemy_manager;
    private bool gave_dest = false;
    public bool prioritize_monster = true;
    // Start is called before the first frame update

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy_manager = GameObject.FindGameObjectWithTag("Enemy_Manager").GetComponent<Enemy_Manager>();
    }

    private void Start()
    {
        if (required_key.Count > 1 && prioritize_monster == true)
        {
            List<int> priority = new();
            foreach (GameObject key in required_key)
            {
                if (key != null)
                {
                    if (key.GetComponent<Key_behaviour>().monster_drop)
                    {
                        priority.Add(required_key.IndexOf(key));
                        //required_key.Remove(key);
                        // required_key.Insert(0, key);
                    }
                }
            }
            foreach (int index in priority)
            {
                GameObject key_to_move = required_key[index];
                required_key.RemoveAt(index);
                required_key.Insert(0, key_to_move);
            }
        }
    }

    public bool CheckKey(GameObject key)
    {
        foreach (GameObject k in required_key)
        {
            if (key == k)
            {
                //open = true;
                return true;
            }
            
        }
        return false;
    }

    private void ResolveDoor(int key_to_check)
    {
        
        if (!unlocked)
        {
            var key = required_key[key_to_check];
            //Debug.Log("Player hit a door , locating possible keys");
            //foreach (GameObject key in required_key)
            //{
                //Debug.Log("Checking for key - " + key.name);
                if (!gave_dest)
                {
                    //Debug.Log("Doesn't have a destination");
                    if (key.GetComponent<Key_behaviour>().monster_drop)
                    {
                        //Debug.Log("Key dropped by monster");
                        foreach (GameObject enemy in enemy_manager.GetAllEnemies())
                        {
                            if (enemy != null && enemy.GetComponent<Enemy_AI>().loot == key)
                            {
                                //Debug.Log("Key dropped by - " + enemy.name);
                                player.GetComponent<AI_Movement>().AddDestination(enemy, 0);
                                gave_dest = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("Key not dropped by monster");
                        if (required_key.Count == 1)
                        {
                            //Debug.Log("unlockable by only one key");
                            player.GetComponent<AI_Movement>().AddDestination(required_key[0], 0);
                            gave_dest = true;
                            //break;
                        }
                        else if (required_key.Count > 1)
                        {
                            //Debug.Log("unlockable by multiple keys");
                            player.GetComponent<AI_Movement>().AddDestination(required_key[keys_failed], 0);
                            player.GetComponent<AI_Movement>().AddDestination(gameObject, 1);
                            gave_dest = true;
                            //break;
                        }
                    }
               // }
            }

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
                        if (player.GetComponent<AI_Movement>().current_destination == gameObject)
                        {
                            player.GetComponent<AI_Movement>().RemoveDestination(gameObject);
                        }
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            ResolveDoor(keys_failed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            gave_dest = false;
        }
    }

    public void KeyFailed()
    {
        Debug.Log("Key Failed");
        keys_failed++;
        gave_dest = false;
        ResolveDoor(keys_failed);
    }
}
