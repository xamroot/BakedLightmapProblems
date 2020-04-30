using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public Transform driverSeat;
    public float deceleration = 0f;
    public float acceleration = 0f;
    public float rotationSpeed = 0f;
    Vector3 driverSeatPosition;
    Vector3 eulerAngles;
    Rigidbody rb;
    float currentAcceleration = 0f;
    float velocity = 0f;
    float maxVelocity = 3f;
    
    void Start()
    {
        if (driverSeat != null)
        {
            driverSeatPosition = driverSeat.position;
        }

        eulerAngles = transform.eulerAngles;

        rb = GetComponent<Rigidbody>();
    }

    public Vector3 GetDriverSeatPosition()
    {
        return driverSeatPosition;
    }

    public void ApplyGas()
    {
        currentAcceleration = acceleration;
    }

    public void VehicleRotator()
    {
        float roty = eulerAngles.y;
        float mouseAcceleration = Input.GetAxis("Mouse X");
        float deltaRot = mouseAcceleration * rotationSpeed * Time.deltaTime;
        if (deltaRot != 0f)
        {
            roty += deltaRot;
        }
        
        transform.rotation = Quaternion.Euler(eulerAngles.x, roty, eulerAngles.z);
        eulerAngles = new Vector3(eulerAngles.x, roty, eulerAngles.z);
    }


    void FixedUpdate()
    {
        if (currentAcceleration <= 0)
        {
            velocity = Mathf.Max(velocity - deceleration * Time.deltaTime, 0f);
            Debug.Log("DECELERATE");
        }
        else
        {
            velocity = Mathf.Min(velocity + currentAcceleration * Time.deltaTime, maxVelocity);
            Debug.Log(velocity);
        }

        currentAcceleration = 0f;
    }

    void Update()
    {
        
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
