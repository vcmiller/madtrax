using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Vector3 velocity;
    SBR.CooldownTimer cd;

    private void Start()
    {
        cd = new SBR.CooldownTimer(1);
    }

    private void FixedUpdate()
    {
        transform.Translate(Time.deltaTime * velocity, Space.World);
        if (cd.Use())
        {
            Destroy(gameObject);
        }
    }
    public void Shoot(Vector3 velocity)
    {
        this.velocity = velocity;
        transform.rotation = Quaternion.LookRotation(Vector3.up, velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        BossHealth bh;
        if(bh = other.GetComponent<BossHealth>())
        {
            bh.Damage(1);
        }
    }
}
