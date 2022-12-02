using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key_behaviour : MonoBehaviour
{
    public bool monster_drop;
   
    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<Player_Movement>().Hold(this.gameObject);
            Debug.Log("Key picked up removing destination");
            player.gameObject.GetComponent<AI_Movement>().Hold(gameObject);
            player.gameObject.GetComponent<AI_Movement>().RemoveDestination(gameObject);
            gameObject.SetActive(false);
        }
    }
}
