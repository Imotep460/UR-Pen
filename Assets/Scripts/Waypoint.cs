using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    public int waypointId;
    public Vector3 waypointPosition;
    public Quaternion waypointRotation;

    public Waypoint(int waypointId, Vector3 waypointPosition, Quaternion waypointRotation)
    {
        this.waypointId = waypointId;
        this.waypointPosition = waypointPosition;
        this.waypointRotation = waypointRotation;
    }

    public Waypoint(Waypoint waypoint)
    {
        this.waypointId = waypoint.waypointId;
        this.waypointPosition = waypoint.waypointPosition;
        this.waypointRotation = waypoint.waypointRotation;
    }
}
