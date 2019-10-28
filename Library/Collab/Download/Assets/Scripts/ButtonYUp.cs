using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonYUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isPressed;
    public static bool MoveyUp;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPressed)
        {
            MoveyUp = false;
            //inputVector = new Vector3(0, 0, 0);
        }
        // Do something here
        if (isPressed == true)
        {
            MoveyUp = true;
            //if (buttonName == "MovexUp")
            //{
            //    inputVector = new Vector3(1 * movementSpeed, player.velocity.y, player.velocity.z);
            //}
            //else if (buttonName == "MoveyUp" && isPressed == true)
            //{
            //    inputVector = new Vector3(player.velocity.x, 1 * movementSpeed, player.velocity.z);
            //}
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
