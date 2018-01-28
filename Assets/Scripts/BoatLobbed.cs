using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatLobbed : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        SBR.Health dmg;
        if(dmg = other.GetComponentInChildren<SBR.Health>())
        {
            dmg.ApplyDamage(new SBR.Damage(2, transform.position, Vector3.up));
        }
    }

}
