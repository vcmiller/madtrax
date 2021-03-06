﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftMotor : BasicMotor<FrancineChannels> {

    //The visible character
    public Transform aestheticTarget;
    public Collider hitbox;
    public AudioSource src;

    public float turnSpeed = 15;
    public float maxTilt = 80f;

    public float dashCooldown = 1;
    public float dashDist = 4;
    public float dashInvuln = 0.2f;
    public float dashDuration = 0.06f;

    private bool dashing;
    private Vector3 preDashVel;

    private CooldownTimer dashTimer;
    private ExpirationTimer dashInvulnTimer;
    private ExpirationTimer dashExpTimer;

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
        dashInvulnTimer = new ExpirationTimer(dashInvuln);
        dashExpTimer = new ExpirationTimer(dashDuration);
    }

    // Update is called once per frame
    public override void TakeInput() {

        //Will is the direction your input WANTS you to go
        Vector3 will = channels.movement2;
        will = Vector3.ClampMagnitude(will, 1);

        //The angle between the will and the direction of the vehicle
        float inputDisparityAngle = Vector3.Cross(Quaternion.Euler(0, 0, 0) * will, aestheticTarget.forward).y;

        //The desired rotation in 2D
        Quaternion targetTilt = aestheticTarget.rotation;
        Quaternion targetRotation = (will.magnitude > 0.01f) ? Quaternion.LookRotation(will, Vector3.up) : transform.rotation;

        //X rotation determines wheelieing, pretty much
        float newX = (Input.GetButton("Fire2")) ? -45f * (motor.velocity.magnitude / motor.walkSpeed) : 0;

        //The desired rotation, now accounting for wheelies and tilt
        targetTilt = Quaternion.Euler(newX, 0, (will.magnitude > 0 ? maxTilt * inputDisparityAngle : 0));

        float f = turnSpeed;

        if (Input.GetButton("Fire2")) {
            f *= 2;
        }

        if (channels.dash && dashTimer.Use()) {
            dashInvulnTimer.Set();
            dashExpTimer.Set();
            dashing = true;
            preDashVel = motor.velocity;
            src.Play();
        }

        if (!dashExpTimer.expired) {
            motor.velocity = aestheticTarget.forward * dashDist / dashDuration;
        } else if (dashing) {
            dashing = false;
            motor.velocity = preDashVel;
        }


        hitbox.enabled = dashInvulnTimer.expired;

        //Apply the rotation to the aesthetic target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, f * Time.deltaTime);
        aestheticTarget.localRotation = Quaternion.RotateTowards(aestheticTarget.localRotation, targetTilt, f * Time.deltaTime);

        //Move forwards, always, but the previous code can change the rotation
        channels.movement += transform.forward * will.magnitude;
    }
}
