using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class EnemyWeapon : MonoBehaviour {

    public float damage;
    public float force;
    
    private void OnTriggerEnter(Collider other) {
        Health bh;

        if (bh = other.GetComponentInParent<Health>()) {
            bh.ApplyDamage(new Damage(damage, transform.position, transform.forward));
        }

        CharacterMotor cm;

        if (cm = other.GetComponentInParent<CharacterMotor>()) {
            cm.velocity += (cm.transform.position - transform.position).normalized * force;
        }
    }
}
