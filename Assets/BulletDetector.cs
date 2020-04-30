using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Bullet")
        {
            Destroy(other.transform.gameObject);
            Destroy(transform.gameObject);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
