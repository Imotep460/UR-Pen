using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Create array of buttons to place the buttons for movement in.
    // Serialize for easy access in the editor.
    [SerializeField]
    private Button[] moveButtons;

    // Create array of buttons to place the buttons for rotating the Pen/Player inside.
    // Serialize for easy access in the editor.
    [SerializeField]
    private Button[] rotateButtons;

    private bool isPressed;

    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(isPressed != true)
        {
            Debug.Log("Button is NOT pressed");
        }
        else if(isPressed == true)
        {
            Debug.Log("Button IS pressed");
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
