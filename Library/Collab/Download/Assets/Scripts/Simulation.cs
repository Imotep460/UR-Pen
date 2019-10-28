using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Simulation : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI positionText;
    [SerializeField]
    private TMPro.TextMeshProUGUI rotationText;

    // Create collection/array of waypoints
    [SerializeField]
    private Transform[] waypoints;
    // Placeholder for the current targetposition
    private Vector3 targetPosition;
    // Placeholder for the current targetrotation
    private Quaternion targetRotation;

    // Create a float to modify the movementspeed of the gameobject.
    [SerializeField]
    private float movementSpeed;

    // Create a float to modify the rotationspeed of the gameobject.
    [SerializeField]
    private float rotationSpeed;

    // Create a int to switch between the waypoints in Transform[] waypoints as well as indicate which waypoint the Player Object is moving to.
    private int waypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        //// Get a path where we can safely store data.
        //Debug.Log(Application.persistentDataPath);
        // Set the first waypoint to move to.
        // Set the first waypoint.position.
        targetPosition = waypoints[0].position;
        // Set the first waypoint.rotation.
        targetRotation = waypoints[0].rotation;
        // make sure the waypointIndex starts at 0.
        waypointIndex = 0;
    }


    // Update is called once per frame
    void Update()
    {
        // Move towards the targetPosition.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        // Rotate towards the targetRotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Print out the current position of the pen tip. Updates every frame.
        positionText.text = string.Format("Position:\n{0}", transform.position);
        // Print ot the current rotation of the pen tip. Updates every frame
        rotationText.text = string.Format("Rotation:\n{0}", transform.rotation);

        // Check if the distance between the gameObjects current position is within a certain distance of the targetPosition before moving on to the next waypoint.
        if (Vector3.Distance(transform.position, targetPosition) < .01)
        {
            // Check to make sure the waypointIndex is NOT biger than the amount of waypoints in Transform[] waypoints.
            if (waypointIndex >= waypoints.Length - 1)
            {
                // If the waypointIndex is bigger than or equal to the total amount of waypoints, then reset the waypoint index to 0. ie: ready to start over.
                waypointIndex = 0;
                // If the waypointIndex is bigger than or equal to the total amount of waypoints, then pause/freeze/stop time/progress.
                Time.timeScale = 0;
            }
            else
            {
                waypointIndex++;
            }
            // Set a new targetPosition.
            targetPosition = waypoints[waypointIndex].position;
            // Set a new targetRotation.
            targetRotation = waypoints[waypointIndex].rotation;
        }
    }
}
