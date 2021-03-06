﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class MohawkBossAttack : BasicMotor<MohawkBossChannels> {

    private MohawkBossImpl mhbImpl;
    private CharacterMotor motor;
    private Animator anim;
    private ExpirationTimer leapTimer;
    private ExpirationTimer chargeTimer;
    private ExpirationTimer damageTimer;
    private ExpirationTimer shootTimer;
    private CooldownTimer shootCooldown;

    private bool isCharging;
    private bool isLeaping;
    private bool showWarn;
    private string curAttack;

    public Transform warnings;

    public GameObject sweepWarn;
    public GameObject chargeWarn;
    public GameObject spinWarn;

    public GameObject sweepCol;
    public GameObject chargeCol;
    public GameObject spinCol;

    public GameObject bulletPrefab;
    public Transform shootPoint;

    public AudioClip attackSound;

    public float fireRate = 60;
    public float chargeSpeed;
    public float chargeTime;
    public float leapTime;
    public float bulletSpeed = 10;
    

    // Use this for initialization
    protected override void Start() {
        base.Start();
        mhbImpl = transform.root.GetComponent<SBR.Brain>().activeController as MohawkBossImpl;
        motor = GetComponentInParent<CharacterMotor>();
        anim = GetComponent<Animator>();

        leapTimer = new ExpirationTimer(leapTime);
        chargeTimer = new ExpirationTimer(chargeTime);
        damageTimer = new ExpirationTimer(1);
        shootCooldown = new CooldownTimer(1 / fireRate);
        shootTimer = new ExpirationTimer(1);

        warnings.parent = null;
    }

    private void OnDestroy() {
        if (warnings) {
            Destroy(warnings.gameObject);
        }
    }

    public override void TakeInput() {

        if (showWarn) {
            if (curAttack == "Leap") {
                ShowSweepWarn();
            } else if (curAttack == "Charge") {
                ShowChargeWarn();
            } else if (curAttack == "Spin") {
                ShowSpinWarn();
            }
        }

        if ((curAttack == "Spin" || curAttack == "Charge") && !showWarn) {
            mhbImpl.TurnTowardsPlayer(mhbImpl.trackSpeed);
        }

        if (channels.doAttack != null) {
            curAttack = channels.doAttack;
            anim.Play(channels.doAttack);

            if (curAttack == "Spin") {
                showWarn = true;
            }
        }

        if (isCharging || isLeaping) {
            //mhbImpl.TurnTowardsPlayer(mhbImpl.turnSpeedCharging);
            motor.velocity = mhbImpl.transform.forward * chargeSpeed;
        }

        if (isLeaping && leapTimer.expired) {
            SetLeaping(0);
        }

        if (isCharging && chargeTimer.expired) {
            SetCharging(0);
        }

        if (!shootTimer.expired && shootCooldown.Use()) {
            Instantiate(bulletPrefab, shootPoint.position, transform.rotation).GetComponent<Bullet>().Shoot(Quaternion.Euler(0, transform.eulerAngles.y + 180 + 180 * (0.5f - shootTimer.remainingRatio), 0) * Vector3.forward * bulletSpeed);
        }
    }

    public void NotifyEndAnim() {
        mhbImpl.endAnimationFlag = true;
        HideWarnings();
    }

    private void ShowSweepWarn() {
        sweepWarn.SetActive(true);
        

        if (!damageTimer.expired) {
            sweepCol.SetActive(true);
            sweepCol.transform.position = sweepWarn.transform.position;
            sweepCol.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y + 135 * damageTimer.remainingRatio);
        } else {
            sweepCol.SetActive(false);
        }
    }

    private void ShowChargeWarn() {
        chargeWarn.SetActive(true);

        if (!chargeTimer.expired) {
            chargeCol.SetActive(true);
            chargeCol.transform.position = chargeWarn.transform.position + chargeWarn.transform.up * (chargeSpeed * chargeTime + 4) * (1 - chargeTimer.remainingRatio);
            chargeCol.transform.localScale = new Vector3(2, 2, 1);
            chargeCol.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        } else {
            chargeCol.SetActive(false);
        }
    }

    private void ShowSpinWarn() {
        spinWarn.SetActive(true);

        spinWarn.transform.position = mhbImpl.transform.position;
        spinWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);

        if (!damageTimer.expired) {
            spinCol.SetActive(true);
            spinCol.transform.position = spinWarn.transform.position;
            spinCol.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y + 315 * (1 - damageTimer.remainingRatio));
        } else {
            spinCol.SetActive(false);
        }
    }

    private void HideWarnings() {
        sweepWarn.SetActive(false);
        spinWarn.SetActive(false);
        chargeWarn.SetActive(false);

        sweepCol.SetActive(false);
        spinCol.SetActive(false);
        chargeCol.SetActive(false);

        showWarn = false;
    }

    public void SetFiring(float duration) {
        shootTimer.expiration = duration;
        shootTimer.Set();
    }

    public void SetDamaging(float duration) {
        if (curAttack == "Spin") {
            Util.PlayClipAtPoint(attackSound, transform.position, 1, 0, false, transform);
        }

        damageTimer.expiration = duration;
        damageTimer.Set();
    }

    public void Track(float time) {
        Vector3 aimToPlayer = PredictPosition(mhbImpl.player.GetComponent<CharacterMotor>(), mhbImpl.transform, time);

        mhbImpl.transform.rotation = Quaternion.LookRotation(aimToPlayer, Vector3.up);
        
        if (curAttack == "Charge") {
            chargeSpeed = (aimToPlayer.magnitude - 2) / chargeTime;

            chargeWarn.transform.position = mhbImpl.transform.position;
            chargeWarn.transform.localScale = new Vector3(2, chargeSpeed * chargeTime / 2 + 4, 1);
            chargeWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        } else if (curAttack == "Leap") {
            chargeSpeed = (aimToPlayer.magnitude - 5) / leapTime;

            sweepWarn.transform.position = mhbImpl.transform.position + mhbImpl.transform.forward * chargeSpeed * leapTime;
            sweepWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        }


        showWarn = true;
    }

    public void SetCharging(int isCharging) {
        if (isCharging == 1) {
            chargeTimer.Set();
            Util.PlayClipAtPoint(attackSound, transform.position, 1, 0, false, transform);
        } else {
            motor.velocity = Vector3.zero;
        }
        this.isCharging = isCharging == 1;
    }

    public void SetLeaping(int isLeaping) {
        if (isLeaping == 1) {
            leapTimer.Set();
            Util.PlayClipAtPoint(attackSound, transform.position, 1, 0, false, transform);
        } else {
            motor.velocity = Vector3.zero;
        }
        this.isLeaping = isLeaping == 1;
    }

    public static Vector3 PredictPosition(CharacterMotor body, Transform reference, float time) {
        Vector3 pos = body.transform.position - reference.transform.position;
        pos.y = 0;

        Vector3 vel = body.velocity;
        
        return pos + vel * time;
        
    }
}
