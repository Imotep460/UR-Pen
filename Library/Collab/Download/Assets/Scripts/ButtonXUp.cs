using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonXUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isPressed = false;
    public static bool MovexUp = false;


    // Update is called once per frame
    void Update()
    {
        if (!isPressed)
        {
            MovexUp = true;
        }
        // Do something here
        if(isPressed == true)
        {
            MovexUp = false;
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
