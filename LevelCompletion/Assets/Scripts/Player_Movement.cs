using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player_Movement : MonoBehaviour
{
    PlayerControl controls;
    CharacterController controller;
    public Vector2 movement_input;
    [SerializeField] private float movement_speed = 20.0f;
    public int number_jumps = 1;
    [SerializeField] private float jump_force = 2.0f;
    [SerializeField] private float jump_velocity = 0.0f;
    [System.NonSerialized] public int jump_attempts = 0;
    public bool jumping = false;
    public bool landing = false;
    [SerializeField] private float gravity = 9.8f;
    private float additional_decay = 0.0f;
    [SerializeField] private float decay_multiplier = 0.2f;
    private GameObject[] doors;
    private GameObject[] portals;
    [SerializeField] private List<GameObject> held_items = new();


    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControl();
        controller = GetComponent<CharacterController>();
        doors = GameObject.FindGameObjectsWithTag("Door");
        portals = GameObject.FindGameObjectsWithTag("Portal");
        controls.Player.Move.performed += ctx => movement_input = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement_input = Vector2.zero;
        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Interact.performed += ctx => Interact();
    }

    // Update is called once per frame
    void Update()
    {
        HandleJump();
        HandleMovement();
        //Debug.Log(held_item);
       
    }
    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            if (jumping && additional_decay >= decay_multiplier)
            {
                //Debug.Log("Landed");
                jumping = false;
                additional_decay = 0.0f;
                jump_attempts = 0;
            }
        }
        if (jumping)
        {
            jump_velocity -= (gravity * Time.deltaTime) + additional_decay;
            additional_decay += (Time.deltaTime * (movement_speed * 0.1f) * decay_multiplier);
        }

    }

    private void HandleMovement()
    {
        Vector3 input_direction = new(movement_input.x, 0.0f, 0.0f);
       
        Vector3 movement = new Vector3();
        if (movement_input.x != 0)
        {
            //movement.x += movement_input.x * movement_speed * Time.deltaTime;
            movement.x = movement_input.x * movement_speed * 4 * Time.deltaTime;
        }
        

        movement.y = jump_velocity;
        var movement_motion = movement_speed * Time.deltaTime * movement;
    
        controller.Move(movement_motion);
    }
    private void Jump()
    {
        //Debug.Log("Jump");
        if (jump_attempts < number_jumps && !landing)
        {
            jumping = true;
            jump_velocity = jump_force;
            additional_decay = 0.0f;
            jump_attempts += 1;
        }
    }

    private void Interact()
    {
        //Debug.Log("Pressed Interact");
        GameObject closest_door = null;
        float closest_dist = 999f;
        foreach( GameObject door in doors)
        {
            if (door != null)
            {//Debug.Log("Looking for closest door...");
                if (Vector2.Distance(transform.position, door.transform.position) < closest_dist)
                {
                    closest_dist = Vector2.Distance(transform.position, door.transform.position);
                    closest_door = door;
                }
            }
        }
        //Debug.Log("Closest door - " + closest_door);
        if (closest_door != null && held_items != null)
        {
            //Debug.Log("has closest door and held item");
            if (Vector2.Distance(transform.position, closest_door.transform.position) < 2f)
            {
                foreach (GameObject held_key in held_items)
                {
                    if (held_items != null)
                    {//Debug.Log("In range, checking key");
                        if (closest_door.GetComponent<Door_logic>().CheckKey(held_key))
                        {
                            //Debug.Log("Correct key");
                            held_items.Remove(held_key);
                            Destroy(closest_door);
                            Destroy(held_key);
                            break;
                        }
                    }
                }
               
            }
        }

        foreach(GameObject portal in portals)
        {
            //Debug.Log("Looking for all portals...");
            if (portal.GetComponent<Portal_behavoiur>().IsActive())
            {
                //Debug.Log("found active portal");
                portal.GetComponent<Portal_behavoiur>().Teleport();
                break;
            }
        }

    }

    public void Hold(GameObject key)
    {
        held_items.Add(key);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }
    public void EnableInput()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    public void DisableInput()
    {
        controls.Player.Disable();
    }
}
