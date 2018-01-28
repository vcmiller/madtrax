using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class MohawkBossDamage : MonoBehaviour {
    private Health health;

    public GameObject phase2Object;
    public float phase2Ratio = 0.5f;

    private void Start() {
        health = GetComponent<Health>();
    }

    void DamageNotify(Damage dmg) {
        if (health.health / health.maxHealth < phase2Ratio) {
            phase2Object.SetActive(true);
        }
    }

    void ZeroHealth() {
        var anim = GetComponentInChildren<Animator>();

        GetComponent<Collider>().enabled = false;
        GetComponent<Brain>().enabled = false;
        phase2Object.SetActive(false);
        anim.Play("Die");
        Destroy(gameObject, 3);
    }
}
