using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableProp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void OnRotate(Vector3 axis, float angle)
    {
        transform.Rotate(axis, angle, Space.World);
    }
}
