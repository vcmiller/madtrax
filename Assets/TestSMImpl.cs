using UnityEngine;

public class TestSMImpl : TestSM {
    public bool patrolIdle;
    public bool patrolPursue;
    public bool idlePatrol;
    public bool pursuePatrol;
    public Vector3 vector;
    public int[] array;

    protected override void StateEnter_Patrol() {
        print("Enter Patrol");
    }

    protected override void State_Patrol() {
        transform.position += Vector3.up * Time.deltaTime;
    }

    protected override void StateExit_Patrol() {
        print("Exit Patrol");
    }

    protected override void StateEnter_Pursue() {
        print("Enter Pursue");
    }

    protected override void State_Pursue() {
        transform.position += Vector3.down * Time.deltaTime;
    }

    protected override void StateExit_Pursue() {
        print("Exit Pursue");
    }

    protected override void StateEnter_Idle() {
        print("Enter Idle");
    }

    protected override void State_Idle() {
    }

    protected override void StateExit_Idle() {
        print("Exit Idle");
    }

    protected override bool TransitionCond_Patrol_Idle() { return patrolIdle; }
    protected override void TransitionNotify_Patrol_Idle() { print("Patrol to Idle");  }
    protected override bool TransitionCond_Patrol_Pursue() { return patrolPursue; }
    protected override void TransitionNotify_Patrol_Pursue() { print("Patrol to Pursue");  }
    protected override bool TransitionCond_Idle_Patrol() { return idlePatrol; }
    protected override void TransitionNotify_Idle_Patrol() { print("Idle to Patrol");  }
    protected override bool TransitionCond_Pursue_Patrol() { return pursuePatrol; }
    protected override void TransitionNotify_Pursue_Patrol() { print("Pursue to Patrol"); }

}
