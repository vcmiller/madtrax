using UnityEngine;
using SBR;

public class FinalBossImpl : FinalBoss {
    public Transform player;
    public GameObject boatThrown;
    public GameObject boatLobbed;

    public Vector3 towardsPlayer
    {
        get
        {
            Vector3 v = player.transform.position - transform.position;
            v.y = 0;
            return v;
        }
    }

    public float riseSpeed;
    public float walkSpeed;
    public float chargeSpeed;

    public float boatThrowInterval;
    public int boatsToThrow;
    private int boatsThrown;
    private CooldownTimer boatThrowTimer;

    public float chargeTime;
    private CooldownTimer chargeTimer;
    private Vector3 chargeDirection;

    public float giveUpTime;
    public float giveUpRadius;
    public float sproutTime;
    private CooldownTimer giveUpChase;
    private CooldownTimer sproutBoats;

    protected override void OnControllerEnabled()
    {
        base.OnControllerEnabled();

        boatThrowTimer = new CooldownTimer(boatThrowInterval);
        chargeTimer = new CooldownTimer(chargeTime);
        giveUpChase = new CooldownTimer(giveUpTime);
        sproutBoats = new CooldownTimer(sproutTime);
    }

    public void TurnTowardsPlayer(float speed)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsPlayer, Vector3.up), speed * Time.deltaTime);
    }

    protected override void State_Rising() {
        transform.position += riseSpeed * Vector3.up * Time.deltaTime;
    }
    protected override void State_ThrowingBoats() {
        if (boatThrowTimer.Use())
        {
            boatsThrown++;
            Instantiate(boatThrown, transform.position, Quaternion.identity);
        }
    }
    protected override void State_Charge() {
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);
    }

    protected override void StateEnter_BoatSprout() {
        for(int i = 0; i < 8; i++)
        {
            Vector3 direction = Quaternion.Euler(0, i * (360 / 8), 0) * (transform.forward);
            GameObject boat = Instantiate(boatLobbed, transform.position + 2 * direction, Quaternion.identity);
            boat.GetComponent<Rigidbody>().AddForce(5 * (direction + Vector3.up), ForceMode.VelocityChange);
        }
    }
    protected override void StateEnter_Charge()
    {
        chargeDirection = towardsPlayer;
        chargeTimer.Reset();
    }

    protected override void State_Chasing() {
        TurnTowardsPlayer(10);
        transform.Translate(towardsPlayer * Time.deltaTime * walkSpeed);
    }

    protected override void StateExit_ThrowingBoats()
    {
        boatsThrown = 0;
    }

    protected override bool TransitionCond_idle_Rising() { return player.position.y <= -480f; }
    protected override bool TransitionCond_Rising_Chasing() { return transform.position.y > 500f; }

    protected override bool TransitionCond_ThrowingBoats_Chasing() { return boatsThrown >= boatsToThrow; }
    protected override bool TransitionCond_Charge_Chasing() { return chargeTimer.Use(); }
    protected override bool TransitionCond_BoatSprout_Chasing() { return FindObjectOfType<BoatLobbed>() != null; }

    protected override bool TransitionCond_Chasing_ThrowingBoats() { return giveUpChase.canUse && towardsPlayer.magnitude > giveUpRadius; }
    protected override bool TransitionCond_Chasing_BoatSprout() { return sproutBoats.Use(); }
    protected override bool TransitionCond_Chasing_Charge() { return giveUpChase.canUse && towardsPlayer.magnitude <= giveUpRadius; }

}
