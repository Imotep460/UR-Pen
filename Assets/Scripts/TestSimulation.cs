using System;
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
    // Cast for the Text object. Meant to represent the x, y, z, w, rotation of the GameObject.
    [SerializeField]
    private TMPro.TextMeshProUGUI rotationText;
    // Cast a field to show the progression throgh the waypoints.
    [SerializeField]
    private TMPro.TextMeshProUGUI pointProgression;
    // Another way of showing the progression through the waypoints.
    [SerializeField]
    private TMPro.TextMeshProUGUI pointProgression2;

    // Cast the index used to navigate through the lists:
    private int waypointIndex;

    // Cast fields for the GameObjects start position and start rotation.
    // startPosition and startRotation is set in TransformSaver.cs
    public static Vector3 startPosition;
    public static Quaternion startRotation;

    // Cast list of vector3 positions
    public List<Vector3> pointsPosition = new List<Vector3>();
    // Cast list of Quaternion rotations;
    public List<Quaternion> pointsRotation = new List<Quaternion>();

    // Cast the target Vector3.
    private Vector3 targetPosition;
    // Cast for the targetRotation/target Quarternion
    private Quaternion targetRotation;

    // Cast for movementSpeed variable to increase or decrease the movement speed of the GameObject.
    // If the movementSpeed is 0 then then GameObject will not move,
    // and if the movementSpeed is negative then the GameObject will never reach it's target position.
    // Therefore recommended that the movementSpeed is kept positive.
    [SerializeField]
    private float movementSpeed;
    // Cast for rotationSpeed variable to increase or decrease the rotation speed of the GameObject.
    [SerializeField]
    private float rotationSpeed;

    // To save the points I Cast a List<string> where I add a string by the following format:
    // string.format(pointPosition.x, pointPosition.y, pointPosition.z, pointRotation.x, pointRotation.y, pointRotation.z, pointRotation.w)
    public static List<string> stringsToSave = new List<string>();

    // Cast the list of loaded points.
    public static List<string> loadedStrings = SavingService.LOADED_POINTS;

    // Cast Object reference to the TrailRendere component on the Pen GameObject,
    // so the TrailRenderer can be scripted/accessed.
    private TrailRenderer tR;

    // Awake is called before any Start methods, Awake (like Start) is called only once.
    private void Awake()
    {
        // Pause the scene/simulation on startup.
        Time.timeScale = 0;
        // Get the TrailRenderer component.
        tR = GetComponent<TrailRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load and format the points from the SaveGame.json file into pointsPosition and pointsRotation lists.
        ReadFromFile();

        // Make sure the waypointIndex is set to 0 at start
        waypointIndex = 0;
        // Set the start vector3 position equal item 0 in pointsPosition list.
        targetPosition = pointsPosition[0];
        // Set the Start Quaternion rotation equal item 0 in pointsRotation list.
        targetRotation = pointsRotation[0];

        // To avoid the TrailRenderer making a trail from the GameObjects position in the editor to it's spawn position on scene load,
        // we quickly clears the TrailRenderer on Start. 
        // NOTE this does not disable the TrailRenderer it merely clears all trails made prior to calling the Clear method.
        tR.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure the TrailRenderer is ON
        tR.emitting = true;

        // Display the current position of the GameObject. The text to appear in in the Text object.
        positionText.text = string.Format("Position:\n{0}", transform.position);
        // Display the current euler rotation of the GameObject. The text to appear in the Text object.
        rotationText.text = string.Format("Rotation:\n{0}", transform.rotation.eulerAngles);

        if (waypointIndex > 0)
        {
            // Display the waypoint progression.
            pointProgression.text = string.Format("Point {0} of {1}", waypointIndex, pointsPosition.Count);
            // Display moving from point a to point b
            pointProgression2.text = string.Format("Point {0} => Point {1}", waypointIndex, waypointIndex + 1);
        }
        else if (waypointIndex == 0)
        {
            pointProgression.text = string.Format("Start position of {0}", pointsPosition.Count);
            pointProgression2.text = string.Format("Start position => Point {0}", waypointIndex + 1);
        }

        // Move towards the targetPosition.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        // Rotate towards the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the GameObject is within a certain distance of the targetPosition
        if (Vector3.Distance(transform.position, targetPosition) < .00001f)
        {
            if (transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else if (transform.rotation == targetRotation)
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
                    // Output to the Unity editor console the current targetposition, targetrotation, and point x of y
                    //Debug.LogFormat("TargetPosition = {0}, targetRotation = {1}, Point {2} of {3}", targetPosition, targetRotation, waypointIndex + 1, pointsPosition.Count);
                }
            }
            // Designate a new target position
            targetPosition = pointsPosition[waypointIndex];
            // Designate a new target rotation
            targetRotation = pointsRotation[waypointIndex];
        }
    }

    /// <summary>
    /// Get the previous waypoints position and rotation.
    /// Move the pen backwards through the points in pointsPosition and pointsRotation lists,
    /// eventually reaching startPosition and startRotation.
    /// </summary>
    public void getPreviousWaypoint()
    {
        Time.timeScale = 0;

        // turn the TrailRenderer off as there has most likely already been created a Trail so no reason to alocate power to create a new trail on top an existing one.
        tR.emitting = false;

        Vector3 tempPoint = pointsPosition[Math.Max(0, waypointIndex - 1)];
        Quaternion tempRotation = pointsRotation[Math.Max(0, waypointIndex - 1)];

        // Check if the GameObject is at the startPosition.
        if (transform.position == startPosition)
        {
            Debug.LogFormat("The GameObject is at the startPosition, nothing to do");

            targetPosition = pointsPosition[0];
            targetRotation = pointsRotation[0];
            waypointIndex = 0;
            return;
        }

        if (waypointIndex <= 0)
        {
            Debug.LogFormat("something something");
            tempPoint = pointsPosition[0];
            tempRotation = pointsRotation[0];
        }
        else if (waypointIndex > 0)
        {
            tempPoint = pointsPosition[waypointIndex - 1];
            tempRotation = pointsRotation[waypointIndex - 1];
        }

        // Check if GameObject is transitioning between 2 points.
        if (transform.position != tempPoint)
        {
            // If transform.position is NOT at tempPoint and waypointIndex is 0 go to startPosition and startRotation.
            // As the GameObject MUST be transitioning from startPosition to pointsPosition[0].
            if (waypointIndex <= 0)
            {
                transform.localPosition = startPosition;
                transform.localRotation = startRotation;

                targetPosition = pointsPosition[0];
                targetRotation = pointsRotation[0];
                waypointIndex = 0;
            }
            // If the waypointIndex is bigger than 0 the GameObject MUST be transitioning between points in the pointsPosition list.
            else if (waypointIndex > 0)
            {
                // Simply move the GameObject back to the previous targetPosition.
                transform.localPosition = pointsPosition[Math.Max(0, waypointIndex - 1)];
                transform.localRotation = pointsRotation[Math.Max(0, waypointIndex - 1)];
            }
        }
        else if (transform.position == tempPoint)
        {

        }
    }

    /// <summary>
    /// Get the next waypoints position and rotation.
    /// </summary>
    public void getNextWaypoint()
    {
        // Check to see if the GameObject is currently transitioning in between 2 points.
        if (transform.position != targetPosition)
        {
            // If the GameObject is transitioning between 2 points instantly go to the targetPosition/targetRotation.
            transform.localPosition = targetPosition;
            transform.localRotation = targetRotation;
        }
        else if (transform.position == targetPosition)
        {
            // If the GameObject is NOT transitioning between 2 points check if the GameObject has reached the end of the list.
            if (waypointIndex < pointsPosition.Count - 1)
            {
                // If the GameObject has not reached the end of the list, simply go to the next point in the list.
                transform.localPosition = pointsPosition[waypointIndex + 1];
                transform.localRotation = pointsRotation[waypointIndex + 1];

                // Increment the waypointIndex so it is up to date with GameObjects position,
                // relative to which point in pointsPosition/pointsRotation the GameObject has reached.
                waypointIndex++;
            }
            else if(waypointIndex >= pointsPosition.Count - 1)
            {
                // If the GameObject has reached the end of the list send a message to the console,
                // and make sure that the waypointIndex does not go out of bounds
                Debug.LogFormat("You have reached the end of the list!");
                waypointIndex = pointsPosition.Count - 1;
            }
        }
    }

    /// <summary>
    /// Reset/reload the scene to the start position of the path. Get the startPosition and startRotation from TransformSaver.cs
    /// </summary>
    public void ResetScene()
    {
        // Reset the position and rotation of the GameObject to the Start position and start rotation.
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        // Reset the waypointIndex
        waypointIndex = 0;
        // Reset the targetPosition and the targetRotation at waypointIndex 0.
        targetPosition = pointsPosition[waypointIndex];
        targetRotation = pointsRotation[waypointIndex];

        // Clear the Trail on reset.
        tR.Clear();
        Time.timeScale = 0;
    }

    /// <summary>
    /// Populate the pointsPosition list with Vector3's and the pointsRotation with Quaternion's with custom data.
    /// Recomended to call this method in Start/Awake, for a smooth startup.
    /// Do NOT use with a working save system.
    /// Use this method to test writing to file.
    /// </summary>
    //void populateLists()
    //{
    //    Vector3 rotationsVector;

    //    // Create custom Vector3's and custom Quaternion's for testing purposes.
    //    pointsPosition.Add(new Vector3(0, 1, 0));
    //    pointsPosition.Add(new Vector3(1, 1, 0));
    //    pointsPosition.Add(new Vector3(1, 1, 1));
    //    pointsPosition.Add(new Vector3(2, 2, 2));
    //    pointsPosition.Add(new Vector3(3, 3, 3));
    //    pointsPosition.Add(new Vector3(0, 0, 0));
    //    pointsPosition.Add(new Vector3((float)7.5, 0, 0));
    //    pointsPosition.Add(new Vector3(14, 0, 0));
    //    pointsRotation.Add(Quaternion.Euler(0, 0, 0));
    //    pointsRotation.Add(Quaternion.Euler(45, 0, 0));
    //    pointsRotation.Add(Quaternion.Euler(0, 0, 0));
    //    // When NOT using Quaternion.Euler remember to convert input to float and add a w with value=1 as seen below.
    //    pointsRotation.Add(new Quaternion((float)0.45, 0, 0, 1));
    //    rotationsVector = new Vector3(25, 0, 0);
    //    pointsRotation.Add(Quaternion.Euler(rotationsVector));
    //    pointsRotation.Add(Quaternion.Euler(0, 0, 0));
    //    pointsRotation.Add(Quaternion.Euler(45, 0, 0));
    //    pointsRotation.Add(Quaternion.Euler(-45, 0, 0));
    //}

    public void buildStringList()
    {
        // Run through the list of points.
        for (int i = 0; i < pointsPosition.Count; i++)
        {
            // Separate the different values in each Vector3 in pointsPosition and Quaternion in pointsRotation
            string xp = pointsPosition[i].x.ToString();
            string yp = pointsPosition[i].y.ToString();
            string zp = pointsPosition[i].z.ToString();
            string xr = pointsRotation[i].x.ToString();
            string yr = pointsRotation[i].y.ToString();
            string zr = pointsRotation[i].z.ToString();
            string wr = pointsRotation[i].w.ToString();

            // Create and format a string and add it to the points list.
            stringsToSave.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", xp.Replace(",", "."), yp.Replace(",", "."), zp.Replace(",", "."), xr.Replace(",", "."), yr.Replace(",", "."), zr.Replace(",", "."), wr.Replace(",", ".")));
            //pointsStringList.Add(string.Format("{0}, {1}", vector, quaternion));
        }
    }

    /// <summary>
    /// Check if TestSimulator.cs loadedStrings contains any strings,
    /// if loadedStrings does contain strings then decrypt these strings into float values,
    /// and create new Vector3 pointsPositions & Quaternion pointsRotations using these values.
    /// </summary>
    /// <returns></returns>
    private bool ReadFromFile()
    {
        // Check if loadedStrings contains any strings.
        if (loadedStrings.Count - 1 < 0)
        {
            Debug.LogErrorFormat("Loading points failed! There is no strings in the 'points' list!");
            return false;
        }
        else
        {
            // Create a local variable to serve as a index for how many strings are present in loadedStrings
            var loadListCount = loadedStrings.Count;

            // Run through the loadedStrings sing loadListCount as a index.
            for (int i = 0; i < loadListCount; i++)
            {
                // Create a temporary variable to hold loadedStrings at index i.
                var pointsTemp = loadedStrings[i];

                // Split pointsTemp into several strings at each "," (1 string = 1 value).
                string[] pointTemp = pointsTemp.Split(","[0]);

                // In order for Unity to be able to properly understand the values in each pointTemp string,
                // we need to replace each "." with a ",".
                float x = float.Parse(pointTemp[0].Replace(".", ","));
                float y = float.Parse(pointTemp[1].Replace(".", ","));
                float z = float.Parse(pointTemp[2].Replace(".", ","));
                float xr = float.Parse(pointTemp[3].Replace(".", ","));
                float yr = float.Parse(pointTemp[4].Replace(".", ","));
                float zr = float.Parse(pointTemp[5].Replace(".", ","));
                float wr = float.Parse(pointTemp[6].Replace(".", ","));

                // Now that Unity can understand our values we can create new Vector3's and Quaternion's.
                pointsPosition.Add(new Vector3(x, y, z));
                pointsRotation.Add(new Quaternion(xr, yr, zr, wr));
            }
            Debug.Log("All data read from file.");
            return true;
        }
    }
}