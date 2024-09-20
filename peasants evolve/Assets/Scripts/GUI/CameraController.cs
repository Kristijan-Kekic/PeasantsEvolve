using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Set these values in the Inspector to define the starting view
    public Vector3 startingPosition;  // The desired starting position of the camera
    public Vector3 startingRotation;  // The desired starting rotation (Euler angles)

    void Start()
    {
        // Set the camera's position and rotation when the game starts
        transform.position = startingPosition;
        transform.rotation = Quaternion.Euler(startingRotation);

        // Optionally, you can print out the starting position and rotation for debugging
        Debug.Log("Camera starting position: " + startingPosition);
        Debug.Log("Camera starting rotation: " + startingRotation);
    }
}
