using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Camera cam;
    public float speed = 0f;
    public float rotationSpeed = 0f;
    public bool canWalk = true;
    Vector3 eulerAngles;

    void Start()
    {
        eulerAngles = transform.eulerAngles;        
    }
    
    public void PlayerRotator()
    {
        float roty = eulerAngles.y;
        float mouseAccelerationX = Input.GetAxis("Mouse X");
        if (mouseAccelerationX != 0f)
        {
            roty += mouseAccelerationX * rotationSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(eulerAngles.x, roty, eulerAngles.z);
        eulerAngles = new Vector3(eulerAngles.x, roty, eulerAngles.z);
        
        float mouseAccelerationY = Input.GetAxis("Mouse Y");
        float rotx = cam.transform.eulerAngles.x;
        if (Mathf.Abs(mouseAccelerationY) > 0.5f)
        {
            rotx -= mouseAccelerationY * rotationSpeed * 0.75f * Time.deltaTime;
        }
        cam.transform.rotation = Quaternion.Euler(rotx, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
    }

    public void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
    }
}
