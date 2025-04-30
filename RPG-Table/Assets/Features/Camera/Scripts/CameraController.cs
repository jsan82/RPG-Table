using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 50.0f;     // Camera speed
    public float rotationSpeed = 10.0f; // Camera rotation speed
    public float zoomSpeed = 50.0f;     // Camera zoom speed
    public float minZoom = 5f;          // Minimal zoom
    public float maxZoom = 1000f;       // Maximal zoom

    private float currentX = 0.0f;      // Current rotation X 
    private float currentY = 0.0f;      // current rotation y
    private float currentDistance = 200.0f; // current zoom

    public float yMinLimit = -90f;      // minimal tile angle
    public float yMaxLimit = 90f;       // maximal tile angle

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
        if (Input.GetKey(KeyCode.Q)) moveY = -1;
        if (Input.GetKey(KeyCode.E)) moveY = 1;

        Vector3 move = new Vector3(moveX, moveY, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.Self); 
    }

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