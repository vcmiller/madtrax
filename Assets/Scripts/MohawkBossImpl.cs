using UnityEngine;
using SBR;

public class MohawkBossImpl : MohawkBoss {

    public Transform player;
    public float beginChargeRadius;
    public float sweepRadius = 3;
    public float turnSpeed = 180;
    public float trackSpeed = 360;
    public float aimDistSpeed = 5;
    public VariantFloat giveUpChaseTime;
    public VariantFloat shotsShotsTime;
    [HideInInspector]
    public Animator animator;
    CooldownTimer giveUpChase;
    CooldownTimer shotTimer;

    private MohawkBossChannels bossChannels;

    public int chargesBeforeLeaping = 2;
    public int spinsBeforeLeaping = 2;

    int consecutiveCharges = 0;
    int consecutiveSpins = 0;

    public bool endAnimationFlag = false;

    public Vector3 towardsPlayer {
        get {
            Vector3 v = player.transform.position - transform.position;
            v.y = 0;
            return v;
        }
    }

    public void TurnTowardsPlayer(float speed) {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsPlayer, Vector3.up), speed * Time.deltaTime);
    }

    protected override void OnControllerEnabled() {
        base.OnControllerEnabled();
        maxTransitionsPerUpdate = 1;
        shotTimer = new CooldownTimer(shotsShotsTime.Evaluate());
        animator = GetComponentInChildren<Animator>();
        bossChannels = channels as MohawkBossChannels;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void TriggerAnimation(string anim) {
        bossChannels.doAttack = anim;
    }

    protected override void StateEnter_Chase() {
        if (bossChannels != null) bossChannels.doAttack = "Crawl";
        endAnimationFlag = false;
        giveUpChase = new CooldownTimer(giveUpChaseTime.Evaluate());
    }
    protected override void State_Chase() {
        TurnTowardsPlayer(turnSpeed);
        bossChannels.movement = transform.forward;
    }
    protected override void StateEnter_Leap() {
        consecutiveCharges = 0;
        consecutiveSpins = 0;
        bossChannels.doAttack = "Leap";
    }
    protected override void StateEnter_ShotsShotsShots() {
        shotTimer = new CooldownTimer(shotsShotsTime.Evaluate());
        consecutiveCharges = 0;
        consecutiveSpins = 0;
        bossChannels.doAttack = "ShotsShots";
    }
    protected override void StateEnter_Charge() {
        consecutiveCharges++;
        consecutiveSpins = 0;
        bossChannels.doAttack = "Charge";
    }
    protected override void StateEnter_Sweep() {
        consecutiveCharges = 0;
        consecutiveSpins++;
        bossChannels.doAttack = "Spin";
    }

    protected override void State_AimTowardsPlayer() {
        State_Chase();
    }

    protected override bool TransitionCond_Chase_Leap() { return giveUpChase.Use() || consecutiveCharges >= chargesBeforeLeaping || consecutiveSpins >= spinsBeforeLeaping; }
    protected override bool TransitionCond_Chase_AimTowardsPlayer() { return shotTimer.Use(); }
    protected override bool TransitionCond_Chase_Charge() {
        return consecutiveCharges < chargesBeforeLeaping && towardsPlayer.magnitude > beginChargeRadius;
    }
    protected override bool TransitionCond_Chase_Sweep() { return towardsPlayer.magnitude < sweepRadius && consecutiveSpins < spinsBeforeLeaping; }

    protected override bool TransitionCond_Leap_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_ShotsShotsShots_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_Charge_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_Sweep_Chase() { return ResetAnimFlag(); }

    protected override bool TransitionCond_AimTowardsPlayer_ShotsShotsShots() {
        Vector3 off = player.position - transform.position;
        Vector3 fwd = transform.forward;

        off.y = 0;
        fwd.y = 0;

        return Vector3.Angle(fwd, off) < 30;
    }

    bool ResetAnimFlag() {
        return endAnimationFlag;
    }

}
