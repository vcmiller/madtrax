using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class Bullet : MonoBehaviour {

    private Vector3 velocity;
    public GameObject impact;
    public bool destroyOnImpact = true;

    private void FixedUpdate()
    {
        transform.Translate(Time.deltaTime * velocity, Space.World);
    }
    public void Shoot(Vector3 velocity)
    {
        this.velocity = velocity;
        transform.rotation = Quaternion.LookRotation(Vector3.up, velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health bh;
        if(bh = other.GetComponentInParent<Health>())
        {
            if (destroyOnImpact) {
                Destroy(gameObject);
            }
            bh.ApplyDamage(new Damage(1, transform.position, velocity));

            if (impact) {
                Instantiate(impact, transform.position, transform.rotation);
            }
        }
    }
}
