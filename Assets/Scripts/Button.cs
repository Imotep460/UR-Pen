using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //public Vector3 input;
    public bool isPressed = false;
    [SerializeField]
    public Rigidbody player;
    [SerializeField]
    private float movementSpeed;

    private Vector3 inputVector;

    void Start()
    {
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPressed)
        {
            Debug.Log("button is NOT pressed!");
            inputVector = new Vector3(0, 0, 0);
        }
        // Do something here
        if(isPressed == true)
        {
            Debug.Log("Button is pressed.");
            inputVector = new Vector3(1 * movementSpeed, player.velocity.y, player.velocity.z);
        }

    }

    void FixedUpdate()
    {
        player.velocity = inputVector;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    //public void BMoveX()
    //{
    //    player = GameObject.FindWithTag("Player");
    //    player.transform.position = new Vector3(1, 0, 0);
    //}
}
