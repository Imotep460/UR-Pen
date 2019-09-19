using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    // Create collection/array of waypoints
    [SerializeField]
    private Transform[] waypoints;
    // Placeholder for the current targetposition
    private Vector3 targetPosition;
    // Placeholder for the current targetrotation
    private Quaternion targetRotation;

    // Create a float to modiy the movementspeed of the gameobject.
    [SerializeField]
    private float movementSpeed;

    // Create a float to modify the rotationspeed of the gameobject.
    [SerializeField]
    private float rotationSpeed;

    // Create a int to switch between the waypoints in Tranform[] waypoints.
    private int waypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        // Set the first waypoint to move to.
        // Set the first waypoint position.
        targetPosition = waypoints[0].position;
        // Set the first waypoint rotation.
        targetRotation = waypoints[0].rotation;
        // make sure the waypointIndex starts a 0.
        waypointIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards the targetPosition.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        // Rotate towards the targetRotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // Check if the distance between the gameObjects current position is within a certain distance of the targetPosition.
        if (Vector3.Distance(transform.position, targetPosition) < .25)
        {
            // Check to make sure the waypointIndex is NOT biger than the amount of waypoints in Transform[] waypoints.
            if (waypointIndex >= waypoints.Length - 1)
            {
                waypointIndex = 0;
            }
            else
            {
                waypointIndex++;
            }
            // Set a new targetPosition.
            targetPosition = waypoints[waypointIndex].position;
            // Set a new targetRotation.
            targetRotation = waypoints[waypointIndex].rotation;
            if (waypointIndex > waypoints.Length - 1)
            {
                // Pause the screen/game/simulation.
                Time.timeScale = 0;
            }
        }
    }
}
