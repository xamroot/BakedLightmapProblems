using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public ParticleSystem MuzzleFlash;
    public GameObject BulletPrefab;
    public float BulletSpeed = 0f;
    public float noise = 50f;
    Transform barrelTip;
    List<AberrationEffect> abEffects;
    int abEffectLength = 0;

    void Start()
    {
        abEffects = new List<AberrationEffect>();
        barrelTip = transform.GetChild(0);

        noise = 50f;
    }

    void Resonance(Vector3 shotPosition, float range)
    {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
        float dist = 0f;
        
        for (int i=0; i<humans.Length; ++i)
        {
            
            dist = Vector3.Distance(humans[i].transform.position, shotPosition);
            
            if (dist <= range)
            {
                humans[i].GetComponent<HumanController>().BecomeCurious(shotPosition);
            }
        }
    
    }

    public void Shoot(Vector3 dir)
    {
        MuzzleFlash.Play();
        
        GameObject bullet = Instantiate(BulletPrefab, barrelTip.position - transform.right * 0.125f, barrelTip.rotation);    
        BulletController b = bullet.GetComponent<BulletController>();
        b.speed = BulletSpeed;

        Resonance(barrelTip.position, noise);
    }

    public void AddAberration(AberrationEffect a)
    {
        abEffects.Add(a);
        ++abEffectLength;
    }

    void Update()
    {
    }
}
