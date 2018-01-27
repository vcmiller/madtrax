using UnityEngine;
using SBR;

public class MohawkBossImpl : MohawkBoss {
    
    public Transform player;
    public float chaseSpeed;
    public float beginAttackRadius;
    public float giveUpChaseTime;
    public float giveUpChaseTimeVariance;
    

    protected override void State_Chase() {
        Vector3 towardsPlayer = (player.position - transform.position);
        transform.Translate(towardsPlayer.normalized * Mathf.Clamp(chaseSpeed, 0, towardsPlayer.magnitude) * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.LookRotation(towardsPlayer, Vector3.up);
    }
    protected override void State_Leap() { }
    protected override void StateEnter_ShotsShotsShots() { }
    protected override void State_ShotsShotsShots() { }
    protected override void State_Charge() { }
    protected override void State_Sweep() { }

    protected override bool TransitionCond_Chase_Leap() { return false; }
    protected override bool TransitionCond_Chase_ShotsShotsShots() { return false; }
    protected override bool TransitionCond_Chase_Charge() { return false; }
    protected override bool TransitionCond_Chase_Sweep() { return false; }
    protected override bool TransitionCond_Leap_Chase() { return false; }
    protected override bool TransitionCond_ShotsShotsShots_Chase() { return false; }
    protected override bool TransitionCond_Charge_Chase() { return false; }
    protected override bool TransitionCond_Sweep_Chase() { return false; }

}
