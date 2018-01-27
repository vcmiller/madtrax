using UnityEngine;
using SBR;

public class MohawkBossImpl : MohawkBoss {
    
    public Transform player;
    public float chaseSpeed;
    public float beginChargeRadius;
    public VariantFloat giveUpChaseTime;
    public VariantFloat shotsShotsTime;
    public Animator animator;
    CooldownTimer giveUpChase;
    CooldownTimer shotTimer;

    public int chargesBeforeSweeping = 2;
    int consecutiveCharges = 0;

    public bool endAnimationFlag = false;

    Vector3 towardsPlayer {
        get {
            return (player.position - transform.position);
        }
    }

    protected override void OnControllerEnabled()
    {
        base.OnControllerEnabled();
        shotTimer = new CooldownTimer(shotsShotsTime.Evaluate());
    }

    void TriggerAnimation(string anim)
    {
        if (anim != "Charge" && anim != "Chase") { consecutiveCharges = 0; }
        //Begin an animation in the appropriate state machine
        animator.Play(anim);
    }


    protected override void StateEnter_Chase()
    {
        print("chase");
        giveUpChase = new CooldownTimer(giveUpChaseTime.Evaluate());
    }
    protected override void State_Chase() {
        transform.Translate(towardsPlayer.normalized * Mathf.Clamp(chaseSpeed, 0, towardsPlayer.magnitude) * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(towardsPlayer, Vector3.up), Vector3.up);
    }
    protected override void StateEnter_Leap() {
        TriggerAnimation("Leap");
    }
    protected override void StateEnter_ShotsShotsShots() {
        shotTimer = new CooldownTimer(shotsShotsTime.Evaluate());
        TriggerAnimation("ShotsShotsShots");
    }
    protected override void StateEnter_Charge()
    {
        consecutiveCharges++;
        TriggerAnimation("Charge");
    }
    protected override void StateEnter_Sweep()
    {
        TriggerAnimation("Sweep");
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

    bool ResetAnimFlag()
    {
        if (endAnimationFlag)
        {
            endAnimationFlag = false;
            return true;
        }
        return false;
    }

}
