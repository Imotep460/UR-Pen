using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // Create a field turnSpeed so that the user can control the speed the camera turns in the scene.
    [SerializeField]
    public float turnSpeed = 4.0f;

    // Create the field for the transform to follow
    public Transform pen;

    // Create a field for the Vector we will use to offset the camera with-
    private Vector3 offset;

    private float targetZoomInMax = 2.0f;
    private float targetZoomOutMax = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        // On start up we position the camera.
        offset = new Vector3(pen.position.x, pen.position.y + 3.11f, pen.position.z + -4.30f);
    }

    // LateUpdate is called every frame, AFTER all Update methods have been called and the Behaviour/script is enabled.
    void LateUpdate()
    {
        // Change offset to a Quaternion since we no loger manually needs to position the camera,
        // instead we want to Rotate around the "pen" object.
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.position = pen.position + offset;
        transform.LookAt(pen.position);
    }
}