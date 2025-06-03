using UnityEngine;

/// <summary>
/// Controls free camera movement, rotation, and zoom using mouse and keyboard.
/// Provides first-person style controls with mouse look and keyboard movement.
/// </summary>
public class FreeCameraController : MonoBehaviour
{
    /// <summary>
    /// Base movement speed of the camera in units per second.
    /// </summary>
    public float moveSpeed = 50.0f;
    
    /// <summary>
    /// Rotation sensitivity for mouse look in degrees per pixel movement.
    /// </summary>
    public float rotationSpeed = 10.0f;
    
    /// <summary>
    /// Zoom speed multiplier for mouse wheel input.
    /// </summary>
    public float zoomSpeed = 50.0f;
    
    /// <summary>
    /// Minimum allowed zoom distance from the focal point.
    /// </summary>
    public float minZoom = 5f;
    
    /// <summary>
    /// Maximum allowed zoom distance from the focal point.
    /// </summary>
    public float maxZoom = 1000f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float currentDistance = 200.0f;

    /// <summary>
    /// Minimum vertical rotation angle (looking down) in degrees.
    /// </summary>
    public float yMinLimit = -90f;
    
    /// <summary>
    /// Maximum vertical rotation angle (looking up) in degrees.
    /// </summary>
    public float yMaxLimit = 90f;

    /// <summary>
    /// Handles per-frame input for camera movement and rotation.
    /// Processes mouse look (right mouse button), keyboard movement (WSAD/QE),
    /// and zoom (mouse wheel) inputs.
    /// </summary>
    void Update()
    {
        // Camera rotation (right mouse)
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);
        }

        // Zoom (mouse wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);

        // Movement (WSAD)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0;

        // Up/Down (Q/E)
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1;
        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1;

        Vector3 move = new Vector3(moveX, moveY, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.Self); 
    }

    /// <summary>
    /// Handles camera rotation and zoom after all Update methods have completed.
    /// Ensures smooth camera movement by applying transformations after regular updates.
    /// </summary>
    void LateUpdate()
    {
        // Setting camera rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.rotation = rotation;

        // Zoom 
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f)
        {
            transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, Space.Self);
        }
    }
}