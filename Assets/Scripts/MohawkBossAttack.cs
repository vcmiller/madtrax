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
    private ExpirationTimer damageTimer;
    private ExpirationTimer shootTimer;
    private CooldownTimer shootCooldown;

    private bool isCharging;
    private bool isLeaping;
    private bool isTracking;
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

    private Vector3 aimToPlayer {
        get {
            Vector3 aim;

            if (PredictPosition(mhbImpl.player.GetComponent<CharacterMotor>(), mhbImpl.transform, chargeSpeed, out aim)) {
                return aim;
            } else {
                return mhbImpl.towardsPlayer;
            }
        }
    }

    public override void TakeInput() {
        if (isTracking) {
            mhbImpl.transform.rotation = Quaternion.RotateTowards(mhbImpl.transform.rotation, Quaternion.LookRotation(aimToPlayer, Vector3.up), mhbImpl.trackSpeed * Time.deltaTime);
            
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

        if (isTracking) {
            chargeSpeed = Mathf.MoveTowards(chargeSpeed, (aimToPlayer.magnitude - 5) / leapTime, mhbImpl.aimDistSpeed * Time.deltaTime);

            sweepWarn.transform.position = mhbImpl.transform.position + mhbImpl.transform.forward * chargeSpeed * leapTime;
            sweepWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        }

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

        if (isTracking) {
            chargeSpeed = Mathf.MoveTowards(chargeSpeed, (aimToPlayer.magnitude - 2) / chargeTime, mhbImpl.aimDistSpeed * Time.deltaTime);

            chargeWarn.transform.position = mhbImpl.transform.position;
            chargeWarn.transform.localScale = new Vector3(2, chargeSpeed * chargeTime / 2 + 4, 1);
            chargeWarn.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, 0);
        }

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
        damageTimer.expiration = duration;
        damageTimer.Set();
    }

    public void SetTracking(int isTracking) {
        this.isTracking = isTracking == 1;

        if (this.isTracking) {
            chargeSpeed = 0;
            showWarn = true;
        }
    }

    public void SetCharging(int isCharging) {
        if (isCharging == 1) {
            chargeTimer.Set();
            SetTracking(0);
            Time.timeScale = 0.25f;
        } else {
            Time.timeScale = 1.0f;
            motor.velocity = Vector3.zero;
        }
        this.isCharging = isCharging == 1;
    }

    public void SetLeaping(int isLeaping) {
        if (isLeaping == 1) {
            leapTimer.Set();
            SetTracking(0);
        } else {
            motor.velocity = Vector3.zero;
        }
        this.isLeaping = isLeaping == 1;
    }

    public static bool PredictPosition(CharacterMotor body, Transform reference, float projSpeed, out Vector3 aim) {
        Vector3 pos = body.transform.position - reference.transform.position;
        pos.y = 0;

        Vector3 vel = body.velocity;

        Vector3 posN = pos.normalized;
        Vector3 velN = vel.normalized;

        float a = (projSpeed * projSpeed) - vel.sqrMagnitude;
        float b = 2 * pos.magnitude * vel.magnitude * Vector3.Dot(-posN, velN);
        float c = -pos.sqrMagnitude;

        float disc = b * b - 4 * a * c;

        if (disc < 0) {
            aim = Vector3.zero;
            return false;
        } else {
            float root = Mathf.Sqrt(disc);

            float time = (-b + root) / (2 * a);

            aim = pos + vel * time;

            return true;
        }

    }
}
