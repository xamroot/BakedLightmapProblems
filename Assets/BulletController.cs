using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public MeshRenderer renderer;
    public Rigidbody rb;
    public float speed = 0f;

    public ParticleSystem BloodPrefab;
    public Vector3 dir;
    ParticleSystem blood;

    int shouldDestroy = 0;

    public void SetDirection(Vector3 d)
    {
        dir = d;
    }

    public void KillHuman(bool isPossessed)
    {
        shouldDestroy = 1;

        //remove rigidbody
        Destroy(rb);
        Destroy(renderer);
        
        // start spewing blood
        blood = Instantiate(BloodPrefab, transform.position, transform.rotation);

        // turn particle system toward player
        GameObject p = GameObject.FindWithTag("Player");

        if (isPossessed)
            blood.transform.LookAt(p.transform.GetChild(0).GetChild(2).transform, Vector3.up); // Get The blood spurt spot of the player    
        else
            blood.transform.LookAt(p.transform, Vector3.up);
        
        blood.Play();
    }

    void Update()
    {
        if (dir == null)
        {
            dir = transform.right;
        }

        if (shouldDestroy == 1 && blood != null)
        {
            if (!blood.isPlaying)
            {
                Destroy(blood);
                Destroy(transform.gameObject);
            }
        }
        if (shouldDestroy == 2)
        {
            Destroy(transform.gameObject);
        }
        else
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }   
    }
}
