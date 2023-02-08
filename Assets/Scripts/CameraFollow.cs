using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float cubeWidth;
    public float cubeHeight;

    // Start is called before the first frame update
    void Start()
    {
        //offset = target.position - transform.position;
    }

    public float threshold = 2;

    private void FixedUpdate()
    {
        Vector3 actuallyTarget = target.transform.position + offset;
        float distY = target.transform.position.y - transform.position.y;
        float distX = target.transform.position.x - transform.position.x;
        if (Mathf.Abs(distY) > cubeHeight / 2)
        {
            Vector3 desiredPosition;
            if (distY < 0)
            {
                desiredPosition = new Vector3(transform.position.x, actuallyTarget.y + cubeHeight / 2, transform.position.z);
            }
            else
            {
                desiredPosition = new Vector3(transform.position.x, actuallyTarget.y - cubeHeight / 2, transform.position.z);
            }
            transform.position = desiredPosition;
        }
        if (Mathf.Abs(distX) > cubeWidth / 2)
        {
            Vector3 desiredPosition;
            if (distX < 0)
            {
                desiredPosition = new Vector3(actuallyTarget.x + cubeWidth / 2, transform.position.y, transform.position.z);
            }
            else
            {
                desiredPosition = new Vector3(actuallyTarget.x - cubeWidth / 2, transform.position.y, transform.position.z);
            }
            transform.position = desiredPosition;
        }
        if(Mathf.Abs(distX) <= cubeWidth / 2 && Mathf.Abs(distY) <= cubeHeight / 2)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, actuallyTarget, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
