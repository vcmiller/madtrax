using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class HotShots : MonoBehaviour {

    public Bullet bulletPrefab;
    public float shotInterval;
    CooldownTimer shotTimer;


    Brain brain;
    public DriftController drift
    {
        get
        {
            return brain.activeController as DriftController;
        }
    }


    // Use this for initialization
    void Start () {
        brain = GetComponent<Brain>();
        shotTimer = new CooldownTimer(shotInterval);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = drift.shootRightInput * Vector3.right + drift.shootFwdInput * Vector3.forward;
        if (velocity.magnitude > 0.01f && shotTimer.Use())
        {
            Instantiate(bulletPrefab, transform.position + velocity.normalized, Quaternion.identity).Shoot(velocity);
        }

    }
}
