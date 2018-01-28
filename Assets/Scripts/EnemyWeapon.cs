using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class EnemyWeapon : MonoBehaviour {

    public float damage;
    
    private void OnTriggerEnter(Collider other) {
        Health bh;

        if (bh = other.GetComponentInParent<Health>()) {
            bh.ApplyDamage(new Damage(1, transform.position, transform.forward));
        }
    }
}
