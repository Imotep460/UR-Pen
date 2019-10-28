using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // Define a player/object you wanna control.
    // Serialize or easy access in the editor.
    [SerializeField]
    public Rigidbody player;
    // To add better/faster control over the speed of the Player object create here a float movementspeed.
    // Serialize for easy manipulation in code or directly in the editor.
    [SerializeField]
    private float movementSpeed;

    public bool moveXUp = ButtonXUp.MovexUp;
    public bool MoveyUp = ButtonYUp.MoveyUp;


    private Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
        player.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        //case moveXUp == false && MoveyUp == false:
        //    inputVector = new Vector3(0, 0, 0);
        //    break;

        if(moveXUp == false && MoveyUp == false)
        {
            //Debug.Log("No button is pressed!");
            inputVector = new Vector3(0, 0, 0);
        }
        if(moveXUp == true && MoveyUp == false)
        {
            inputVector = new Vector3(1 * movementSpeed, player.velocity.y, player.velocity.z);
        }
        if(moveXUp == false && MoveyUp == true)
        {
            inputVector = new Vector3(player.velocity.x, 1 * movementSpeed, player.velocity.z);
        }
    }

    private void FixedUpdate()
    {
        player.velocity = inputVector;
    }

    //public void MoveXUp(bool xUp)
    //{
    //    moveXUp = xUp;
    //}
}
