using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePath : MonoBehaviour
{
    
    public float rotationSpeed = 0f;
    public float moveSpeed = 0f;
    enum LINESTATE
    {
        WAIT,
        FORWARD,
        BACKWARD
    }
    LINESTATE state;
    Vector3 startPos;
    Vector3 targetPos;
    Vector3 targetRotationEulers;
    Vector3 currentRotationEulers;
    float lineLength = 0;

    bool running = true;
    void Start()
    {
        state = LINESTATE.WAIT;
       
    }

    public void InitPath(float l)
    {
        lineLength = l;

        startPos = transform.position;
        targetPos = transform.position + transform.forward * lineLength;

        currentRotationEulers = transform.eulerAngles;
        targetRotationEulers = transform.eulerAngles; 

        state = LINESTATE.FORWARD;
    }

    public void StopPath()
    {
        running = false;
    }

    public void ResumePath()
    {
        running = true;
    }

    // Update is called once per frame
    void MoveForward(float spd)
    {  
        transform.position += transform.forward * spd * Time.deltaTime;
    }

    bool CheckTurnDistance(Vector3 a, Vector3 b)
    {
        float d = Vector3.Distance(a, b);
        float maxDist = Vector3.Distance(startPos, targetPos);
        if (d <= 0.05f || d > maxDist)
        {
            return true;
        }
        return false;
    }

    bool CheckEulerDistance(Vector3 a, Vector3 b)
    {
        float d = Vector3.Distance(a, b);
        if (d < 0.05f)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        switch(state)
        {
            case LINESTATE.WAIT:
                break;

            case LINESTATE.FORWARD:
                if (CheckTurnDistance(transform.position, targetPos))
                {
                    // when the human hasn't started turning
                    if (targetRotationEulers == currentRotationEulers)
                    {
                        targetRotationEulers = new Vector3(0f, currentRotationEulers.y + 180f, 0f);
                    
                        // ensure position is accurate
                        transform.position = targetPos;
                    }
                    
                    // human has turned full amount, time to move next direction
                    if (CheckEulerDistance(transform.eulerAngles, targetRotationEulers))
                    {
                        transform.eulerAngles = targetRotationEulers; // ensure rot is accurate
                        currentRotationEulers = transform.eulerAngles; // reset the movement dir to the target movement dir
                        state = LINESTATE.BACKWARD;
                    }
                    // rotate the human towards the target direction to move towards
                    else
                    {
                        Vector3 newRot = Vector3.Lerp(transform.eulerAngles, targetRotationEulers, 0.25f);
                        transform.rotation = Quaternion.Euler(newRot.x, newRot.y, newRot.z);
                    }
                }
                else
                {
                    MoveForward(moveSpeed);
                }

                break;

            case LINESTATE.BACKWARD:

                if (CheckTurnDistance(transform.position, startPos))
                {
                    // when the human hasn't started turning
                    if (CheckEulerDistance(transform.eulerAngles, targetRotationEulers))
                    {
                        targetRotationEulers = new Vector3(0f, currentRotationEulers.y - 180f, 0f);

                        // ensure position is accurate
                        transform.position = startPos;
                    }
                    
                    // human has turned full amount, time to move next direction
                    if (CheckEulerDistance(transform.eulerAngles, targetRotationEulers))
                    {
                        transform.eulerAngles = targetRotationEulers; // ensure rot is accurate
                        currentRotationEulers = transform.eulerAngles; // reset the movement dir to the target movement dir
                        state = LINESTATE.FORWARD;
                    }
                    // rotate the human towards the target direction to move towards
                    else
                    {
                        Vector3 newRot = Vector3.Lerp(transform.eulerAngles, targetRotationEulers, 0.25f);
                        transform.rotation = Quaternion.Euler(newRot.x, newRot.y, newRot.z);
                    }
                }
                else
                {
                    MoveForward(moveSpeed);
                }
                break;
        }
    }
}
