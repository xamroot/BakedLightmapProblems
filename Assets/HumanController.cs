using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public float rotationSpeed = 0f;
    public float moveSpeed = 0f;
    public float killTimer = 0f;
    public float shootCooldownTimer = 0f;
    public float investigateTimer = 0f;
    public float bulletSpeed = 0f;
    public LinePath linePath;
    public SquarePath squarePath;
    public GameObject weapon;
    public Vector3 bulletOffset;
    public enum HUMANSTATE
    {
        NONE,
        LINE,
        SQUARE,
        POSSESSED,
        RESUME_PATROL,
        ALERT,
        INVESTIGATE,
        DEAD
    }
    public HUMANSTATE state;
    HUMANSTATE prevState;
    HUMANSTATE lastCriticalState;
    GameObject currentPath;
    GameObject player;
    Transform lastCriticalTransform;
    Vector3 investigatePosition;
    float shootCooldownCounter = 1f;
    float investigateCounter = 1f;
    float killCounter = 0f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        prevState = HUMANSTATE.NONE;
    }

    public Transform GetGunSpot()
    {
        for (int i=0; i<transform.childCount; ++i)
        {
            if (transform.GetChild(i).name == "GunSpot")
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }
    public void BecomeCurious(Vector3 position)
    {
        if (state != HUMANSTATE.DEAD && state != HUMANSTATE.ALERT 
            && state != HUMANSTATE.RESUME_PATROL && state != HUMANSTATE.POSSESSED)
        {
            StopCurrentPath(prevState);  // path gets stopped a bit early here 

            investigatePosition = position;

            lastCriticalState = state;
            lastCriticalTransform = transform;

            state = HUMANSTATE.INVESTIGATE;
        }
    }

    public void Possess(Transform p)
    {
        transform.parent = p;

        transform.tag = "Possessed";

        lastCriticalState = state;
        state = HUMANSTATE.POSSESSED;
    }

    public void Depossess()
    {
        transform.parent = null;
        
        transform.tag = "Human";

        state = HUMANSTATE.RESUME_PATROL;
    }

    void StopCurrentPath(HUMANSTATE prev)
    {
        switch (prev)
        {
            case HUMANSTATE.LINE:
                linePath.StopPath();
                break;

            case HUMANSTATE.SQUARE:
                squarePath.StopPath();
                break;

            default:
                break;
        }
    }

    bool Sight()
    {
        Vector3 pos = transform.position;

        if (Physics.Raycast(pos, transform.forward, out RaycastHit hit, 3f))
        {
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }
        else if (Physics.Raycast(pos, transform.forward + transform.right * 0.3f, out RaycastHit hit1, 3f))
        {
            if (hit1.transform.tag == "Player")
            {
                return true;
            }
        }
        else if (Physics.Raycast(pos, transform.forward - transform.right * 0.3f, out RaycastHit hit2, 3f))
        {
            if (hit2.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    void OnCollisionEnter(Collision other)
    {
        
        if (other.transform.tag == "Bullet")
        {
            if (state != HUMANSTATE.POSSESSED)
            {
                other.transform.gameObject.GetComponent<BulletController>().KillHuman(false);

                killCounter = killTimer;

                state = HUMANSTATE.DEAD;
            }
            else
            {
                other.transform.gameObject.GetComponent<BulletController>().KillHuman(true);
                // the player would be the parent in this case (possession)
                transform.parent.gameObject.GetComponent<PlayerController>().GetShot();
                transform.parent = null;
                state = HUMANSTATE.DEAD;
            }
        }
    }

    void Update()
    {
        switch(state)
        {
            case HUMANSTATE.LINE:
                if (prevState != HUMANSTATE.LINE)
                {
                    currentPath = linePath.gameObject;

                    linePath.moveSpeed = moveSpeed;
                    linePath.rotationSpeed = rotationSpeed;
                    linePath.InitPath(1f);
                }

                if (Sight())
                {
                    lastCriticalTransform = transform;
                    StopCurrentPath(prevState);  
                    lastCriticalState = state;

                    state = HUMANSTATE.ALERT;
                }

                break;
            
            case HUMANSTATE.SQUARE:
                if (prevState != HUMANSTATE.SQUARE)
                {
                    currentPath = linePath.gameObject;

                    squarePath.moveSpeed = moveSpeed;
                    squarePath.rotationSpeed = rotationSpeed;
                    squarePath.InitPath(1f);
                }

                if (Sight())
                {
                    lastCriticalTransform = transform;
                    StopCurrentPath(prevState);  
                    lastCriticalState = state;

                    state = HUMANSTATE.ALERT;
                }
                break;
            
            case HUMANSTATE.POSSESSED:
                if (prevState != HUMANSTATE.POSSESSED)
                {
                    lastCriticalTransform = transform;
                    StopCurrentPath(prevState);                 
                }
                break;

            case HUMANSTATE.RESUME_PATROL:
                Debug.Log(lastCriticalTransform.position);
                Debug.Log(lastCriticalState);
                if (Vector3.Distance(transform.position, lastCriticalTransform.position) < 0.001f)
                {
                    transform.position = lastCriticalTransform.position;

                    Vector3 e = lastCriticalTransform.eulerAngles;
                    transform.rotation = Quaternion.Euler(0f, (e.y % 360), 0f);
                    
                    state = lastCriticalState;
                    //restart the path
                    switch (state)
                    {
                        case HUMANSTATE.LINE:   
                            linePath.ResumePath();
                            break;

                        case HUMANSTATE.SQUARE:   
                            squarePath.ResumePath();
                            break;

                        default:
                            break;
                    }
                    
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, lastCriticalTransform.position, 0.1f);                   
                }
                break;

            case HUMANSTATE.ALERT:
                transform.LookAt(GameObject.FindWithTag("Player").transform);

                float d = Vector3.Distance(transform.position, player.transform.position);
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, d))
                {
                    if (hit.transform.tag == "Untagged")
                    {
                        state = HUMANSTATE.RESUME_PATROL;
                    }
                    // check if friendly is in line of fire
                    else if (hit.transform.tag == "Human")
                    {
                        float m = 0.05f;
                        float distToLeft = Vector3.Distance(transform.position - transform.right * m, hit.transform.position);
                        float distToRight = Vector3.Distance(transform.position + transform.right * m, hit.transform.position);
                        if (distToLeft >= distToRight)
                            transform.position -= transform.right * m * 2f; // NOTE:need to change from always right at some point
                        else
                            transform.position += transform.right * m * 1.5f;

                        if (shootCooldownCounter < Mathf.Floor(shootCooldownTimer / 2))
                            shootCooldownCounter = shootCooldownTimer;
                    }
                }                

                if (weapon != null)
                {
                    if (shootCooldownCounter <= 0)
                    {   
                        var head = transform.position - player.transform.position; 
                        var direction = head / head.magnitude;
                        weapon.GetComponent<GunController>().BulletSpeed = bulletSpeed;
                        weapon.GetComponent<GunController>().Shoot(direction);
                        
                        shootCooldownCounter = shootCooldownTimer;
                    }
                    else
                    {
                        shootCooldownCounter -= 1f * Time.deltaTime;
                    }
                }
                break;

            case HUMANSTATE.DEAD:
                if (killCounter <= 0)
                {
                    Destroy(transform.gameObject);
                }
                else
                {
                    killCounter -= 1f * Time.deltaTime;
                }

                break;

            case HUMANSTATE.INVESTIGATE:
                // look towards curious spot(maybe add move towards?)
                transform.LookAt(GameObject.FindWithTag("Player").transform);

                if (investigateCounter <= 0)
                {
                    state = HUMANSTATE.RESUME_PATROL;
                }
                else if (Sight())
                {
                    // path should already be stopped

                    // reset rotation?
                    Vector3 e = lastCriticalTransform.eulerAngles;
                    transform.rotation = Quaternion.Euler(e.x, e.y, e.z);

                    state = HUMANSTATE.ALERT;
                }
                else
                {
                    investigateCounter -= 1f * Time.deltaTime;    
                }

                break;
        }

        prevState = state;
    }

    
}
