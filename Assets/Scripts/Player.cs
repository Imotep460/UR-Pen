using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // to have access to a field in the Unity editor a [SerializeField] is required.
    [SerializeField]
    public Rigidbody playerBody;
    // I wanna be able to increase/decrease the move speed of my player object in the editor.
    [SerializeField]
    public float movementSpeed;

    //public GameObject player;

    public Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        //playerBody = GetComponent<Rigidbody>();
        //player = GameObject.FindWithTag("Player");
        //playerBody.useGravity = false;
    }

    // Update is called once per frame ie, this is called as fast a posible.
    //public void Update()
    //{
    //    // Get the 3 dimensional vector that the player controlled object must move in.
    //    // GetAxisRaw goes 0 - 1f instantly if using GetAxis it scales from 0 - 1f slowly useful if a single quick button press takes you shorter than a longer buttonpress.
    //    // check Edit->Project Settings->Input in the Unity Editor to see which key is bound to what axis.
    //    inputVector = new Vector3(Input.GetAxisRaw("Horizontal") * movementSpeed, Input.GetAxisRaw("Jump") * movementSpeed, Input.GetAxisRaw("Vertical") * movementSpeed);
    //}
    // FixedUpdate is called at 50 frames pr second no matter how strong/fast a computer is
    //public void FixedUpdate()
    //{
    //    // to add velocity/force to the player movement playerBody.velocity must be set.
    //    playerBody.velocity = inputVector;
    //}

    //public Vector3 MoveX()
    //{
    //    return inputVector = player.transform.position = new Vector3();
    //}
}
