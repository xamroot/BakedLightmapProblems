using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePath : MonoBehaviour
{
    public float rotationSpeed = 0f;
    public float moveSpeed = 0f;
    enum SQUARESTATE
    {
        WAIT,
        FORWARD
    }
    SQUARESTATE state;
    Vector3[] targetPositions;
    Vector3 targetEulers;
    Vector3 currentEulers;
    int targetIndex = 0;
    float lineLength = 0;
    bool running = true;
    void Start()
    {
        state = SQUARESTATE.WAIT;

        targetPositions = new Vector3[4];
    }

    public void InitPath(float l)
    {
        lineLength = l;

        targetPositions[0] = transform.position;
        
        targetPositions[1] = transform.position + transform.forward * lineLength;
        targetPositions[2] = targetPositions[1] + transform.right * lineLength;
        targetPositions[3] = targetPositions[2] - transform.forward * lineLength;

        currentEulers = transform.eulerAngles;
        targetEulers = transform.eulerAngles; 

        state = SQUARESTATE.FORWARD;
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
        float maxDist = Vector3.Distance(targetPositions[targetIndex], targetPositions[(targetIndex + 1) % 4]);
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
            case SQUARESTATE.WAIT:
                break;

            case SQUARESTATE.FORWARD:
                if (CheckTurnDistance(transform.position, targetPositions[(targetIndex + 1)%4]))
                {
                    // when the human hasn't started turning
                    if (targetEulers == currentEulers)
                    {
                        targetEulers = new Vector3(0f, (currentEulers.y + 90f) % 360, 0f);
                    
                        // ensure position is accurate
                        transform.position = targetPositions[(targetIndex + 1)%4];
                    }
                    
                    // human has turned full amount, time to move next direction
                    if (CheckEulerDistance(transform.eulerAngles, targetEulers))
                    {
                        transform.eulerAngles = targetEulers; // ensure rot is accurate
                        currentEulers = transform.eulerAngles; // reset the movement dir to the target movement dir

                        targetIndex = (targetIndex + 1) % 4;
                    }
                    // rotate the human towards the target direction to move towards
                    else
                    {
                        Vector3 newRot = Vector3.Lerp(transform.eulerAngles, targetEulers, 0.25f);
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
