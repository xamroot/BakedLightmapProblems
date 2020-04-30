using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashDestroyer : MonoBehaviour
{
    float destroyCounter = -1f;
    float destroyTimer = 0f;
    Transform barrelTip;
    public void SetDestroyTimer(float t, Transform b)
    {
        barrelTip = b;
        destroyTimer = t;
        destroyCounter = t;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyTimer != 0)
        {
            if (destroyCounter <= 0)
            {
                Destroy(transform.gameObject);
            }
            else
            {
                Vector3 s = transform.localScale; 
                transform.position = barrelTip.position;
                //transform.localScale = new Vector3(s.x + 0.5f * Time.deltaTime, s.y + 0.5f * Time.deltaTime, s.z + 0.5f * Time.deltaTime);
                destroyCounter -= 1f * Time.deltaTime;
            }
        }
    }
}
