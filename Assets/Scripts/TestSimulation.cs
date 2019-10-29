using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimulation : MonoBehaviour
{
    // Field for the Text object. Meant to represent the x, y, z position of the GameObject.
    // Serialized in order to make it easy to set in the editor.
    // Drag a TextMeshPro text object to this field in the inspector
    [SerializeField]
    private TMPro.TextMeshProUGUI positionText;
    // Field for the Text object. Meant to represent the x, y, z, w, rotation of the GameObject.
    [SerializeField]
    private TMPro.TextMeshProUGUI rotationText;

    // Index used to navigate through the lists:
    private int waypointIndex;
    // Create list of vector3 positions
    private List<Vector3> pointsPosition = new List<Vector3>();
    // Create list of Quaternion rotations;
    private List<Quaternion> pointsRotation = new List<Quaternion>();

    // Get the target Vector3.
    private Vector3 targetPosition;
    // Field for the targetRotation/target Quarternion
    private Quaternion targetRotation;

    // Field for movementSpeed variable to increase or decrease the movement speed of the GameObject.
    // If the movementSpeed is 0 then then GameObject will not move,
    // and if the movementSpeed is negative then the GameObject will never reach it's target position.
    // Therefore recommended that the movementSpeed is kept positive.
    [SerializeField]
    private float movementSpeed;
    // Field for rotationSpeed variable to increase or decrease the rotation speed of the GameObject.
    [SerializeField]
    private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Call to populateLists method at start to populate the ppintsPosition and pointsRotation lists.
        populateLists();
        // Make sure the waypointIndex is set to 0 at start
        waypointIndex = 0;

        // Set the start vector3 position equal item 0 in pointsPosition list.
        targetPosition = pointsPosition[0];
        // Set the Start Quaternion rotation equal item 0 in pointsRotation list.
        targetRotation = pointsRotation[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Display the current position of the GameObject. The text to appear in in the Text object.
        positionText.text = string.Format("Position:\n{0}", transform.position);
        // Display the current euler rotation of the GameObject. The text to appear in the Text object.
        rotationText.text = string.Format("Rotation:\n{0}", transform.rotation.eulerAngles);

        // Move towards the targetPosition.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        // Rotate towards the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the GameObject is within a certain distance of the targetPosition
        if (Vector3.Distance(transform.position, targetPosition) < .00001f)
        {
            new WaitForSeconds(1);
            // Check to make sure that the waypointIndex is NOT bigger than than the maximum number of vector3's in pointsPosition.
            if (waypointIndex >= pointsPosition.Count - 1)
            {
                // If the waypointIndex is bigger than or equal to the total number of Vector3's in pointsPosition then reset the waypointIndex to 0.
                waypointIndex = 0;
                // If the waypointIndex is bigger than or equal to the total number of Vector3's in pointsPosition then pause the game.
                Time.timeScale = 0;
            }
            else
            {
                waypointIndex++;
            }
            // Set a new target position
            targetPosition = pointsPosition[waypointIndex];
            // Set a new target rotation
            targetRotation = pointsRotation[waypointIndex];
        }
    }

    /// <summary>
    /// Get the previous waypoints position and rotation.
    /// </summary>
    void getPreviousWaypoint()
    {
        // Check to see if waypointIndex is 0 and user therefore at beginning of list
        if (waypointIndex > 0)
        {
            waypointIndex--;
            targetPosition = pointsPosition[waypointIndex];
            targetRotation = pointsRotation[waypointIndex];
        }
        else if (waypointIndex <= 0)
        {
            Debug.LogFormat("You are at the beginning of the list!");
            waypointIndex = 0;
        }
    }

    /// <summary>
    /// Get the next waypoints position and rotation.
    /// </summary>
    void getNextWaypoint()
    {
        // Check if the user has reached the final point in the pointsPosition list, and therefore the end of the line.
        if (waypointIndex <= pointsPosition.Count - 1)
        {
            waypointIndex++;
            targetPosition = pointsPosition[waypointIndex];
            targetRotation = pointsRotation[waypointIndex];
        }
        else if (waypointIndex >= pointsPosition.Count - 1)
        {
            Debug.LogFormat("You are at the end of the list!");
            waypointIndex = pointsPosition.Count - 1;
        }
    }

    /// <summary>
    /// Populate the pointsPosition list with Vector3's and the pointsRotation with Quaternion's 
    /// </summary>
    void populateLists()
    {
        Vector3 rotationsVector;

        pointsPosition.Add(new Vector3(0, 1, 0));
        pointsPosition.Add(new Vector3(1, 1, 0));
        pointsPosition.Add(new Vector3(1, 1, 1));
        pointsPosition.Add(new Vector3(2, 2, 2));
        pointsPosition.Add(new Vector3(3, 3, 3));
        pointsPosition.Add(new Vector3(0, 0, 0));
        pointsPosition.Add(new Vector3((float)7.5, 0, 0));
        pointsPosition.Add(new Vector3(14, 0, 0));
        pointsRotation.Add(Quaternion.Euler(0, 0, 0));
        pointsRotation.Add(Quaternion.Euler(45, 0, 0));
        pointsRotation.Add(Quaternion.Euler(0, 0, 0));
        // When NOT using Quaternion.Euler remember to convert input to float and add a w with value=1 as seen below.
        pointsRotation.Add(new Quaternion((float)0.45, 0, 0, 1));
        rotationsVector = new Vector3(25, 0, 0);
        pointsRotation.Add(Quaternion.Euler(rotationsVector));
        pointsRotation.Add(Quaternion.Euler(0, 0, 0));
        pointsRotation.Add(Quaternion.Euler(45, 0, 0));
        pointsRotation.Add(Quaternion.Euler(-45, 0, 0));
    }

    //void generateLists()
    //{
    //    var vectors : Vector3[];

    //    var inputFile = "";//(read the string from the file any way you want, could be www)
    //    var lines = inputFile.Split("\n"[0]); // Gets all lines into seperate strings.
    //    vectors = new Vector3[lines.Length];
    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        var pt = lines[i].Split(","[0]); // Gets 3 parts of the vector into seperate strings.
    //        var x = float.Parse(pt[0]);
    //        var y = float.Parse(pt[1]);
    //        var z = float.Parse(pt[2]);
    //        vectors[i] = new Vector3(x, y, z);
    //    }
    //}
}