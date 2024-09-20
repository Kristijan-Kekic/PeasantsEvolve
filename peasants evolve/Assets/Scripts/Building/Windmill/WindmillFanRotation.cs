using UnityEngine;

public class WindmillFanRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;  // Speed of rotation in degrees per second
    public Vector3 rotationAxis = Vector3.forward;  // Axis to rotate around (Z-axis, assuming forward is along Z)

    void Update()
    {
        // Rotate the windmill fan around the specified axis at a constant speed
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}