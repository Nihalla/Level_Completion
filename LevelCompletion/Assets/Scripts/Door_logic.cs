using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_logic : MonoBehaviour
{
    //private bool open = false;
    public GameObject required_key;
    // Start is called before the first frame update


    public bool CheckKey(GameObject key)
    {
        if(key == required_key)
        {
            //open = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
