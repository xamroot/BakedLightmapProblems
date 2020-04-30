using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamiliarController : MonoBehaviour
{
    public float rotSpeed = 0f;
    public float moveSpeed = 0f;
    public float yPositionLock = 0.5f;
    enum FAMILIARSTATE {
        LINGER,
        ROTATE,
        RUSH
    }
    FAMILIARSTATE state;
    PlayerController target;
    float rotTimer = 1f;
    float rotCounter = 0f;
    float rushTimer = 3f;
    float rushCounter = 0f;
    
    void Start()
    {
        state = FAMILIARSTATE.LINGER;
        target = FindObjectOfType<PlayerController>();
    }

    void LookTowardTarget()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float step = rotSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yPositionLock, transform.position.z);

        switch (state)
        {
            case FAMILIARSTATE.LINGER:
                state = FAMILIARSTATE.ROTATE;
                rotCounter = rotTimer;
                break;

            case FAMILIARSTATE.ROTATE:
                if (rotCounter <= 0)
                {
                    state = FAMILIARSTATE.RUSH;
                    rushCounter = rushTimer;
                }
                else
                {
                    rotCounter -= 1f * Time.deltaTime;
                    LookTowardTarget();
                }
                break;

            case FAMILIARSTATE.RUSH:
                if (rushCounter <= 0)
                {
                    state = FAMILIARSTATE.LINGER;
                }
                else
                {
                    rushCounter -= 1f * Time.deltaTime;
                    MoveForward();
                }
                break;
            

        }
        LookTowardTarget();
    }
}
