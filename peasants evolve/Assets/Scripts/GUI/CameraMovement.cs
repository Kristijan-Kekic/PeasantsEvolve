using UnityEngine;

[RequireComponent(typeof(Camera))] 

public class CameraMovement : MonoBehaviour
{
    public float keyboardSpeed, dragSpeed, screenEdgeSpeed, screenEdgeBorderSize, mouseRotationSpeed, followMoveSpeed, followRotationSpeed,
                 minHeight, maxHeight, zoomSensitivity, zoomSmoothing, mapLimitSmoothing;

    public Vector2 mapLimits, rotationLimits;
    public Vector3 followOffset;

    Transform targetToFollow;
    float zoomAmount = 1, yaw, pitch;
    KeyCode dragKey = KeyCode.Mouse2;
    KeyCode rotationKey = KeyCode.Mouse1;
    private Transform mainTransform;
    LayerMask groundMask;

    private float xMin, xMax, zMin, zMax;


    // Start is called before the first frame update
    void Start()
    {
        mainTransform = transform;
        groundMask = LayerMask.GetMask("Ground");
        pitch = mainTransform.eulerAngles.x;

        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 terrainPosition = terrain.transform.position;

            // Calculate the boundaries based on the terrain position and size, subtracting 100 units for the margin
            xMin = terrainPosition.x + 50f;
            xMax = terrainPosition.x + terrainSize.x - 50f;
            zMin = terrainPosition.z + 50f;
            zMax = terrainPosition.z + terrainSize.z - 50f;

            // Set the camera's starting position to the center of the terrain
            float startX = terrainPosition.x + terrainSize.x / 2f;
            float startZ = terrainPosition.z + terrainSize.z / 2f;
            mainTransform.position = new Vector3(startX, maxHeight, startZ);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetToFollow) { Move(); } else { FollowTarget(); }

        Rotation();
        HeightCalculation();
        LimitPosition();

        if(Input.GetKey(KeyCode.Escape)) { ResetTarget(); }
    }

    void Move()
    {
        if (Input.GetKey(dragKey))
        {
            Vector3 desiredDragMove = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * dragSpeed;
            desiredDragMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredDragMove * Time.deltaTime;
            desiredDragMove = mainTransform.InverseTransformDirection(desiredDragMove);

            mainTransform.Translate(desiredDragMove, Space.Self);
        }
        else
        {
            Vector3 desiredMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            desiredMove *= keyboardSpeed;
            desiredMove *= Time.deltaTime;
            desiredMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredMove;
            desiredMove = mainTransform.InverseTransformDirection(desiredMove);

            mainTransform.Translate(desiredMove, Space.Self);

            //screen edge move
            Vector3 desiredEdgeMove = new Vector3();
            Vector3 mousePos = Input.mousePosition;

            Rect leftRect = new Rect(0, 0, screenEdgeBorderSize, Screen.height);
            Rect rightRect = new Rect(Screen.width - screenEdgeBorderSize, 0, screenEdgeBorderSize, Screen.height);
            Rect upRect = new Rect(0, Screen.height - screenEdgeBorderSize, Screen.width, screenEdgeBorderSize);
            Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorderSize);

            desiredEdgeMove.x = leftRect.Contains(mousePos) ? -1 : rightRect.Contains(mousePos) ? 1 : 0;
            desiredEdgeMove.z = upRect.Contains(mousePos) ? 1 : downRect.Contains(mousePos) ? -1 : 0;

            desiredEdgeMove *= screenEdgeSpeed;
            desiredEdgeMove *= Time.deltaTime;
            desiredEdgeMove = Quaternion.Euler(new Vector3(0, mainTransform.eulerAngles.y, 0)) * desiredEdgeMove;
            desiredEdgeMove = mainTransform.InverseTransformDirection(desiredEdgeMove);

            mainTransform.Translate(desiredEdgeMove, Space.Self);

        }
    }

    void Rotation()
    {
        if (Input.GetKey(rotationKey))
        {
            yaw += mouseRotationSpeed * Input.GetAxis("Mouse X");
            pitch -= mouseRotationSpeed * Input.GetAxis("Mouse Y");

            pitch = Mathf.Clamp(pitch, rotationLimits.x, rotationLimits.y);

            mainTransform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
    }

    void LimitPosition()
    {
        // Clamping the camera position within the calculated boundaries
        float clampedX = Mathf.Clamp(mainTransform.position.x, xMin, xMax);
        float clampedZ = Mathf.Clamp(mainTransform.position.z, zMin, zMax);

        // Directly set the clamped position to enforce strict limits
        mainTransform.position = new Vector3(clampedX, mainTransform.position.y, clampedZ);
    }

    void HeightCalculation()
    {
        zoomAmount += -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;
        zoomAmount = Mathf.Clamp01(zoomAmount);

        float distanceToGround = DistanceToGround();
        float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomAmount);

        mainTransform.position = Vector3.Lerp(mainTransform.position,
            new Vector3(mainTransform.position.x, targetHeight + distanceToGround, mainTransform.position.z), Time.deltaTime * zoomSmoothing);
    }

    private float DistanceToGround()
    {
        Ray ray = new Ray(mainTransform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask)) { return hit.point.y; }
        return 0;
    }

    void FollowTarget()
    {
        Vector3 targetPos = new Vector3(targetToFollow.position.x, mainTransform.position.y, targetToFollow.position.z) + followOffset;
        mainTransform.position = Vector3.MoveTowards(mainTransform.position, targetPos, Time.deltaTime * followMoveSpeed);

        if(followRotationSpeed > 0 && !Input.GetKey(rotationKey))
        {
            Vector3 targetDirection = (targetToFollow.position - mainTransform.position).normalized;
            Quaternion targetRotation = Quaternion.Lerp(mainTransform.rotation, Quaternion.LookRotation(targetDirection), followRotationSpeed * Time.deltaTime);
            mainTransform.rotation = targetRotation;

            pitch = mainTransform.eulerAngles.x;
            yaw = mainTransform.eulerAngles.y;
        }
    }

    public void SetTarget(Transform target)
    {
        targetToFollow = target;
    }

    public void ResetTarget()
    {
        targetToFollow = null;
    }

}
