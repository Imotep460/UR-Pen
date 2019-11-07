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
    // Field for the Text object. Meant to represent the x, y, z, w, rotation of the GameObject.
    [SerializeField]
    private TMPro.TextMeshProUGUI rotationText;

    // Index used to navigate through the lists:
    private int waypointIndex;

    // Create list of vector3 positions
    public List<Vector3> pointsPosition = new List<Vector3>();
    // Create list of Quaternion rotations;
    public List<Quaternion> pointsRotation = new List<Quaternion>();

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

    // To save the points I create a List<string> where I add a string by the following format:
    // string.format(pointPosition.x, pointPosition.y, pointPosition.z, pointRotation.x, pointRotation.y, pointRotation.z, pointRotation.w)
    public static List<string> stringsToSave = new List<string>();

    // Get the list of loaded points.
    public static List<string> loadedStrings = SavingService.LOADED_POINTS;

    // Awake is called before any Start methods, Awake (like Start) is called only once.
    private void Awake()
    {
        Time.timeScale = 0;        
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadFromFile();
        // Call to populateLists method at start to populate the ppintsPosition and pointsRotation lists.
        //populateLists();
        new WaitForSeconds(1);
        // Build the list of strings so it's ready to be saved to the Json savefile later.
        buildStringList();
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
            targetPosition = pointsPosition[0];
            targetRotation = pointsRotation[0];
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
            targetPosition = pointsPosition[pointsPosition.Count - 1];
            targetRotation = pointsRotation[pointsRotation.Count - 1];
        }
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

    private bool ReadFromFile()
    {
        // Check if loadedStrings contains any strings.
        if (loadedStrings.Count - 1 < 0)
        {
            Debug.LogWarningFormat("There is no strings in the list!");
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

                Debug.LogFormat("vector = {0}, Quaternion = {1}", pointsPosition[i].ToString(), pointsRotation[i].ToString());
            }
            Debug.Log("All data read from file.");
            return true;
        }
    }
}