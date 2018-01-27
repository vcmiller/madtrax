using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class HotShots : MonoBehaviour {

    public Bullet bulletPrefab;
    public float shotInterval;
    public Transform gun;
    public DriftRotator driftRotator;
    public float bulletSpeed = 100;  //The speed of bullets
    public float gunTurnSpeed;      //The speed at which the gun can turn
    public float gunTurnPermission; //The angle the gun can rotate in either direction

    public bool autoLockOn;

    public Transform aestheticTarget
    {
        get
        {
            return driftRotator.aestheticTarget;
        }
    }
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

        //Measure shot direction
        Vector3 shotVelocity = drift.shootRightInput * Vector3.right + drift.shootFwdInput * Vector3.forward;

        Quaternion newGunRotation;
        //Determine rotation target and shooting activity
        if (shotVelocity.magnitude > 0.01f) 
        {
            if (autoLockOn)
            {
                newGunRotation = Quaternion.LookRotation(Vector3.up, FindObjectOfType<BossHealth>().transform.position - gun.position);
            }
            else
            {
                newGunRotation = Quaternion.LookRotation(Vector3.up, shotVelocity.normalized);
            }

            if (shotTimer.Use())
            {
                Instantiate(bulletPrefab, gun.position + 3 * gun.up, Quaternion.identity).Shoot(gun.up * bulletSpeed);
            }
        }
        else
        {
            newGunRotation = Quaternion.LookRotation(Vector3.up, aestheticTarget.forward);
        }


        //Rotate gun toward desired direction
        float gunRotationDelta = Time.deltaTime * Vector3.Angle(gun.forward, shotVelocity) * gunTurnSpeed;
        gun.rotation = Quaternion.RotateTowards(gun.rotation, newGunRotation, gunRotationDelta);



        //Clamps gun to permissions
        float gunAngle = Vector3.Angle(gun.up, aestheticTarget.forward);

        if(gunAngle > gunTurnPermission)
        {
            float rotationSign = -Mathf.Sign(Vector3.Cross(gun.up, aestheticTarget.forward).y);
            gun.rotation = Quaternion.LookRotation(Vector3.up, Quaternion.Euler(0, rotationSign * gunTurnPermission, 0) * aestheticTarget.forward);
        }
    }
}
