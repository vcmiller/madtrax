using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftMotor : BasicMotor<FrancineChannels> {

    //The visible character
    public Transform aestheticTarget;

    public float turnSpeed = 15;
    public float maxTilt = 80f;

    public float dashCooldown = 1;
    public float dashDist = 4;

    private CooldownTimer dashTimer;

    public CharacterMotor motor { get; private set; }
    Brain brain;

    public DriftController drift {
        get {
            return brain.activeController as DriftController;
        }
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        brain = GetComponent<Brain>();
        motor = GetComponent<CharacterMotor>();
        dashTimer = new CooldownTimer(dashCooldown);
    }

    // Update is called once per frame
    public override void TakeInput() {

        //Will is the direction your input WANTS you to go
        Vector3 will = channels.movement2;
        will = Vector3.ClampMagnitude(will, 1);

        //The angle between the will and the direction of the vehicle
        float inputDisparityAngle = Vector3.Cross(Quaternion.Euler(0, 0, 0) * will, aestheticTarget.forward).y;

        //The desired rotation in 2D
        Quaternion targetRotation = (will.magnitude > 0.01f) ? Quaternion.LookRotation(will, Vector3.up) : aestheticTarget.rotation;

        //X rotation determines wheelieing, pretty much
        float newX = (Input.GetButton("Fire2")) ? -45f * (motor.velocity.magnitude / motor.walkSpeed) : 0;

        //The desired rotation, now accounting for wheelies and tilt
        targetRotation = Quaternion.Euler(newX, targetRotation.eulerAngles.y, (will.magnitude > 0 ? maxTilt * inputDisparityAngle : 0));

        float f = turnSpeed;

        if (Input.GetButton("Fire2")) {
            f *= 2;
        }

        if (channels.dash && dashTimer.Use()) {
            var capsule = GetComponent<CapsuleCollider>();

            Vector3 p1, p2;
            float radius, height;

            capsule.GetPoints(out p1, out p2, out radius, out height);

            RaycastHit hit;

            Vector3 off = aestheticTarget.forward * -0.5f;

            p1 += capsule.transform.up * 0.2f;
            p2 += capsule.transform.up * 0.2f;
            
            if (Physics.CapsuleCast(p1 - off, p2 - off, radius, aestheticTarget.forward, out hit, dashDist + 1)) {
                transform.position += aestheticTarget.forward * (hit.distance - 1);
            } else {
                transform.position += aestheticTarget.forward * dashDist;
            }

            
        }

        //Apply the rotation to the aesthetic target
        aestheticTarget.rotation = Quaternion.RotateTowards(aestheticTarget.rotation, targetRotation, f * Time.deltaTime);

        //Move forwards, always, but the previous code can change the rotation
        channels.movement += aestheticTarget.forward * will.magnitude;
    }
}
