using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointDatabase : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();

    // Before any Start methods are called run the Awake method thus make sure that the WaypointDatabase is created as one of the first things in the scene.
    private void Awake()
    {
        // Build and populate the database.
        BuildWaypointDatabase();
    }

    // The following method is a method to get a waypoint by it's id.
    public Waypoint GetWaypoint(int id)
    {
        return waypoints.Find(waypoint => waypoint.waypointId == id);
    }

    // Build database method. Create custom waypoints here for test purposes.
    void BuildWaypointDatabase()
    {
        waypoints = new List<Waypoint>()
        {
            // new waypoint = int waypointId, Vector3 waypointPosition, Quaternion waypointRotation.
            new Waypoint(0,
            new Vector3(1, 2, 3),
            new Quaternion(0, 0, 0, 1)),
            new Waypoint(1,
            new Vector3(2, 2, 3),
            new Quaternion(45, 0, 0, 1)),
        };
    }
}
