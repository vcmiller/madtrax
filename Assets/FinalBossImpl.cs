using UnityEngine;
using SBR;

public class FinalBossImpl : FinalBoss {
    public Transform player;
    public GameObject boatThrown;
    public GameObject boatLobbed;
    public float riseSpeed;
    public float walkSpeed;
    public float chaseSpeed;

    protected override void State_Rising() {
        transform.position += riseSpeed * Vector3.up * Time.deltaTime;
    }
    protected override void State_ThrowingBoats() { }
    protected override void State_Charge() { }
    protected override void StateEnter_BoatSprout() { }
    protected override void State_Chasing() { }

    protected override bool TransitionCond_idle_Rising() { return player.position.y <= -480f; }
    protected override bool TransitionCond_Rising_Chasing() { return transform.position.y > 500f; }

    protected override bool TransitionCond_ThrowingBoats_Chasing() { return false; }
    protected override bool TransitionCond_Charge_Chasing() { return false; }
    protected override bool TransitionCond_BoatSprout_Chasing() { return false; }

    protected override bool TransitionCond_Chasing_ThrowingBoats() { return false; }
    protected override bool TransitionCond_Chasing_BoatSprout() { return false; }
    protected override bool TransitionCond_Chasing_Charge() { return false; }

}
