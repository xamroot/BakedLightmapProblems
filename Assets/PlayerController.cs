using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GunController gunCtrl;
    public float startTimer = 1f;
    public float snatchDistance = 0f;
    enum PLAYERSTATE {
        START,
        ONFOOT,
        NEWBODY,
        DEAD
    }
    PLAYERSTATE state;
    HumanController hostBody;
    VehicleController vehicleCtrl;
    MovementController movementCtrl;

    OverlayController overlayCtrl;

    float startCounter = 0f;
    bool canMove = false;

    void Start()
    {
        state = PLAYERSTATE.START;
        vehicleCtrl = FindObjectOfType<VehicleController>();
        movementCtrl = GetComponent<MovementController>();

        startCounter = startTimer;
        transform.rotation = Quaternion.Euler(0,0,0);

        overlayCtrl = FindObjectOfType<OverlayController>();
    }

    bool CanEnterVehicle()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit vehicleHit, 1f))
        {
            if (vehicleHit.transform.tag == "Vehicle")
            {
                return true;
            }
        }
        return false;
    }

    bool BodySnatch()
    {        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit0, snatchDistance))
        {
            if (hit0.transform.tag == "Human")
            {
                state = PLAYERSTATE.NEWBODY;
                if (hostBody != null)
                {
                    if (gunCtrl != null)
                    {
                        gunCtrl.transform.parent = null;

                        Transform hostGunSpot = hostBody.GetGunSpot();
                        gunCtrl.transform.position = hostGunSpot.position;
                        gunCtrl.transform.rotation = Quaternion.Euler(hostGunSpot.eulerAngles.x, hostGunSpot.eulerAngles.y, hostGunSpot.eulerAngles.z);
                        gunCtrl.transform.parent = hostBody.transform;
                    }
                    //handle letting go of the current snatched body
                    hostBody.Depossess();                    

                    gunCtrl = null;
                }
                hostBody = hit0.transform.GetComponent<HumanController>();
                return true;
            }
            
        }
        return false;
    }

    public void GetShot()
    {
        state = PLAYERSTATE.DEAD;
    }

    void FixedUpdate()
    {
        canMove = true;
    }

    void Update()
    {
        switch (state)
        {
            case PLAYERSTATE.START:
                if (startCounter <= 0f)
                {
                    state = PLAYERSTATE.ONFOOT;
                }
                else
                {
                    startCounter -= 1f * Time.deltaTime;
                }
                break;

            case PLAYERSTATE.ONFOOT:
                movementCtrl.Movement();
                movementCtrl.PlayerRotator();

                if (Input.GetMouseButtonDown(0) && gunCtrl != null)
                {
                    gunCtrl.Shoot(transform.forward);
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    if (BodySnatch())
                    {
                        // play particle system for snatching (NOTE main camera always child 0)
                        transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().Play();

                        return;
                    }
                }

                break;
            
            case PLAYERSTATE.NEWBODY:
                Vector3 curPos = transform.position;
                Vector3 newPos = hostBody.transform.position;
                Vector3 newRot = hostBody.transform.eulerAngles;

                if (Vector3.Distance(curPos, newPos) <= 0.125f)
                {
                    // STOP particle system for snatching (NOTE main camera always child 0)
                    transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().Stop();

                    transform.position = hostBody.transform.position;
                    hostBody.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);

                    // note that main camera is always the first child of the player
                    if (hostBody.weapon != null)
                    {
                        hostBody.weapon.transform.parent = transform.GetChild(0);  
                        gunCtrl = hostBody.weapon.GetComponent<GunController>();

                        hostBody.weapon.transform.position = Vector3.zero;
                        hostBody.weapon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        hostBody.weapon.transform.position = transform.GetChild(0).GetChild(0).position;  
                        //hostBody.weapon.transform.rotation = transform.GetChild(0).GetChild(0).rotation;
                        hostBody.weapon.transform.rotation = Quaternion.Euler(Vector3.zero);
                        hostBody.weapon.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    }

                    
                    //transform.rotation = Quaternion.Euler(Vector3.zero);
                    transform.position = hostBody.transform.position + transform.forward * 0.05f;
                    hostBody.Possess(transform);
                    state = PLAYERSTATE.ONFOOT;
                }

                transform.position = Vector3.Lerp(curPos, newPos, 0.3f);
                break;
            
            case PLAYERSTATE.DEAD: 
                if (Input.GetKey(KeyCode.E))
                {
                    SceneManager.LoadScene("SampleScene");;
                }

                overlayCtrl.TurnOnRestartText();

                break;
        }
    }
}
