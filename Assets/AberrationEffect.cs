using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AberrationEffect : MonoBehaviour
{
    public string Effect = "SLOTH";
    public MeshRenderer renderer;
    public Rigidbody rb;
    public BoxCollider box;
    public MeshFilter filter;
    public GunController gunCtrl;
    bool collected = false;
    public void Aberration(GameObject bullet)
    {
        bullet.transform.localScale *= 3f;
        bullet.GetComponent<BulletController>().speed *= 0.5f;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player" && !collected)
        {
            gunCtrl.AddAberration(this);
            Destroy(filter);
            Destroy(rb);
            Destroy(box);
            Destroy(renderer);

            collected = true;
        }
    }
}
