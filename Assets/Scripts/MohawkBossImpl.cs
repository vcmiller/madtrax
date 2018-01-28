using UnityEngine;
using SBR;

public class MohawkBossImpl : MohawkBoss {

    public Transform player;
    public float chaseSpeed;
    public float beginChargeRadius;
    public float turnSpeed = 180;
    public float trackSpeed = 360;
    public float turnSpeedCharging = 90;
    public VariantFloat giveUpChaseTime;
    public VariantFloat shotsShotsTime;
    [HideInInspector]
    public Animator animator;
    CooldownTimer giveUpChase;
    CooldownTimer shotTimer;

    private MohawkBossChannels bossChannels;

    public int chargesBeforeSweeping = 2;
    int consecutiveCharges = 0;

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
        bossChannels.doAttack = "Leap";
    }
    protected override void StateEnter_ShotsShotsShots() {
        shotTimer = new CooldownTimer(shotsShotsTime.Evaluate());
        consecutiveCharges = 0;
        bossChannels.doAttack = "ShotsShots";
    }
    protected override void StateEnter_Charge() {
        consecutiveCharges++;
        bossChannels.doAttack = "Charge";
    }
    protected override void StateEnter_Sweep() {
        consecutiveCharges = 0;
        bossChannels.doAttack = "Spin";
    }

    protected override bool TransitionCond_Chase_Leap() { return giveUpChase.Use(); }
    protected override bool TransitionCond_Chase_ShotsShotsShots() { return shotTimer.Use(); }
    protected override bool TransitionCond_Chase_Charge() {
        return consecutiveCharges < chargesBeforeSweeping && towardsPlayer.magnitude < beginChargeRadius;
    }
    protected override bool TransitionCond_Chase_Sweep() { return consecutiveCharges >= chargesBeforeSweeping; }

    protected override bool TransitionCond_Leap_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_ShotsShotsShots_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_Charge_Chase() { return ResetAnimFlag(); }
    protected override bool TransitionCond_Sweep_Chase() { return ResetAnimFlag(); }

    protected override void TransitionNotify_Chase_Sweep() {
    }

    bool ResetAnimFlag() {
        return endAnimationFlag;
    }

}
