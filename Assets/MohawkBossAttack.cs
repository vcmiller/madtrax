using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class MohawkBossAttack : BasicMotor<MohawkBossChannels> {

    private MohawkBossImpl mhbImpl;
    private CharacterMotor motor;
    private Animator anim;
    private ExpirationTimer leapTimer;
    private ExpirationTimer chargeTimer;

    private bool isCharging;
    private bool isLeaping;
    private bool isTracking;
    private bool showWarn;
    private string curAttack;
    private bool isDamaging;

    public GameObject sweepWarn;
    public GameObject chargeWarn;
    public GameObject spinWarn;

    public GameObject sweepCol;
    public GameObject chargeCol;
    public GameObject spinCol;

    public float chargeSpeed;
    public float chargeTime;
    public float leapTime;
    

    // Use this for initialization
    protected override void Start() {
        base.Start();
        mhbImpl = transform.root.GetComponent<SBR.Brain>().activeController as MohawkBossImpl;
        motor = GetComponentInParent<CharacterMotor>();
        anim = GetComponent<Animator>();

        leapTimer = new ExpirationTimer(leapTime);
        chargeTimer = new ExpirationTimer(chargeTime);
    }

    public override void TakeInput() {
        if (isTracking) {
            mhbImpl.TurnTowardsPlayer(mhbImpl.trackSpeed);
        }

        if (showWarn) {
            if (curAttack == "Leap") {
                ShowSweepWarn();
            } else if (curAttack == "Charge") {
                ShowChargeWarn();
            } else if (curAttack == "Spin") {
                ShowSpinWarn();
            }
        }

        if (channels.doAttack != null) {
            isDamaging = false;
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
    }

    public void NotifyEndAnim() {
        mhbImpl.endAnimationFlag = true;
        HideWarnings();
    }

    private void ShowSweepWarn() {
        sweepWarn.SetActive(true);

        if (isTracking) {
            chargeSpeed = (mhbImpl.towardsPlayer.magnitude - 5) / leapTime;

            sweepWarn.transform.position = mhbImpl.transform.position + mhbImpl.transform.forward * chargeSpeed * leapTime;
            sweepWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        }
    }

    private void ShowChargeWarn() {
        chargeWarn.SetActive(true);

        if (isTracking) {
            chargeSpeed = (mhbImpl.towardsPlayer.magnitude - 2) / chargeTime;

            chargeWarn.transform.position = mhbImpl.transform.position;
            chargeWarn.transform.localScale = new Vector3(1, chargeSpeed * chargeTime / 2 + 4, 1);
            chargeWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        }
    }

    private void ShowSpinWarn() {
        spinWarn.SetActive(true);

        spinWarn.transform.position = mhbImpl.transform.position;
        spinWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
    }

    private void HideWarnings() {
        sweepWarn.SetActive(false);
        spinWarn.SetActive(false);
        chargeWarn.SetActive(false);
        showWarn = false;
    }

    public void SetDamaging(int isDamaging) {
        this.isDamaging = isDamaging == 1;
    }

    public void SetTracking(int isTracking) {
        this.isTracking = isTracking == 1;

        if (this.isTracking) {
            showWarn = true;
        }
    }

    public void SetCharging(int isCharging) {
        if (isCharging == 1) {
            chargeTimer.Set();
            SetDamaging(1);
            SetTracking(0);
        } else {
            motor.velocity = Vector3.zero;
        }
        this.isCharging = isCharging == 1;
    }

    public void SetLeaping(int isLeaping) {
        if (isLeaping == 1) {
            leapTimer.Set();
            SetDamaging(1);
            SetTracking(0);
        } else {
            motor.velocity = Vector3.zero;
        }
        this.isLeaping = isLeaping == 1;
    }

}
