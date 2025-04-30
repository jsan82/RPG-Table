using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;         
    public float distance = 200.0f;    
    public float zoomSpeed = 50.0f;    
    public float minDistance = 5f;    
    public float maxDistance = 1000f;  

    public float rotationSpeed = 10.0f; 
    private float currentX = 0.0f;
    private float currentY = 20.0f;     

    public float yMinLimit = 0f;       
    public float yMaxLimit = 1000f;

    public float panSpeed = 50f;
    public Vector2 panLimit;

    void Update()
    {
        if (target == null)
            return;

        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveZ) * panSpeed * Time.deltaTime;
        target.Translate(move, Space.World);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

}