using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject look_at;
    private float offset = 2f;
    private float player_move;
    [SerializeField] private float smoothing;
    private Vector3 offset_pos;
    // Start is called before the first frame update
    void Start()
    {
        player_move = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>().movement_input.x;
        transform.position = new Vector3(look_at.transform.position.x, look_at.transform.position.y, -10);
    }

    // Update is called once per frame
    void Update()
    {
        player_move = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>().movement_input.x;
        //Debug.Log(player_move);
        if(player_move > 0)
        {
            offset_pos = new Vector3(look_at.transform.position.x + offset, look_at.transform.position.y, -10);
        }
        else if (player_move < 0)
        {
            offset_pos = new Vector3(look_at.transform.position.x - offset, look_at.transform.position.y, -10);
        }

        transform.position = Vector3.Lerp(transform.position, offset_pos, smoothing * Time.deltaTime);
    }
}
