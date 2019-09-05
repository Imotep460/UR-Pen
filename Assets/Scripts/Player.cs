using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // to have access to a field in the Unity editor a [SerializeField] is required.
    [SerializeField]
    private Rigidbody playerBody;
    // I wanna be able to increase/decrease the move speed of my player object in the editor.
    [SerializeField]
    private float movementSpeed;

    private Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        //playerBody.useGravity = false;
    }

    // Update is called once per frame ie, this is called as fast a posible.
    void Update()
    {
        // Get the 3 dimensional vector that the player controlled object must move in.
        // GetAxisRaw goes 0 - 1f instantly if using GetAxis it scales from 0 - 1f slowly useful if a single quick button press takes you shorter than a longer buttonpress.
        // check Edit->Project Settings->Input in the Unity Editor to see which key is bound to what axis.
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal") * movementSpeed, Input.GetAxisRaw("Jump") * movementSpeed, Input.GetAxisRaw("Vertical") * movementSpeed);
    }
    // FixedUpdate is called at 50 frames pr second no matter how strong/fast a computer is
    private void FixedUpdate()
    {
        // to add velocity/force to the player movement playerBody.velocity must be set.
        playerBody.velocity = inputVector;
    }

    public void moveX()
    {
        //playerController.transform.position += ;
    }
}
