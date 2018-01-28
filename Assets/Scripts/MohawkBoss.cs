using UnityEngine;
using SBR;

public abstract class MohawkBoss : SBR.StateMachine {
    public enum StateID {
        Chase, Leap, ShotsShotsShots, Charge, Sweep
    }

    private class State {
        public StateID id;

        public Notify enter;
        public Notify during;
        public Notify exit;

        public Transition[] transitions;
    }

    private class Transition {
        public State from;
        public State to;

        public Notify notify;
        public Condition cond;
    }

    private State[] states;
    private State currentState;

    public MohawkBoss() {
        states = new State[5];

        State stateChase = new State();
        stateChase.id = StateID.Chase;
        stateChase.enter = StateEnter_Chase;
        stateChase.during = State_Chase;
        stateChase.transitions = new Transition[4];
        states[0] = stateChase;

        State stateLeap = new State();
        stateLeap.id = StateID.Leap;
        stateLeap.enter = StateEnter_Leap;
        stateLeap.transitions = new Transition[1];
        states[1] = stateLeap;

        State stateShotsShotsShots = new State();
        stateShotsShotsShots.id = StateID.ShotsShotsShots;
        stateShotsShotsShots.enter = StateEnter_ShotsShotsShots;
        stateShotsShotsShots.transitions = new Transition[1];
        states[2] = stateShotsShotsShots;

        State stateCharge = new State();
        stateCharge.id = StateID.Charge;
        stateCharge.enter = StateEnter_Charge;
        stateCharge.transitions = new Transition[1];
        states[3] = stateCharge;

        State stateSweep = new State();
        stateSweep.id = StateID.Sweep;
        stateSweep.enter = StateEnter_Sweep;
        stateSweep.transitions = new Transition[1];
        states[4] = stateSweep;

        currentState = stateChase;

        Transition transitionChaseLeap = new Transition();
        transitionChaseLeap.from = stateChase;
        transitionChaseLeap.to = stateLeap;
        transitionChaseLeap.cond = TransitionCond_Chase_Leap;
        stateChase.transitions[0] = transitionChaseLeap;

        Transition transitionChaseShotsShotsShots = new Transition();
        transitionChaseShotsShotsShots.from = stateChase;
        transitionChaseShotsShotsShots.to = stateShotsShotsShots;
        transitionChaseShotsShotsShots.cond = TransitionCond_Chase_ShotsShotsShots;
        stateChase.transitions[1] = transitionChaseShotsShotsShots;

        Transition transitionChaseCharge = new Transition();
        transitionChaseCharge.from = stateChase;
        transitionChaseCharge.to = stateCharge;
        transitionChaseCharge.cond = TransitionCond_Chase_Charge;
        stateChase.transitions[2] = transitionChaseCharge;

        Transition transitionChaseSweep = new Transition();
        transitionChaseSweep.from = stateChase;
        transitionChaseSweep.to = stateSweep;
        transitionChaseSweep.cond = TransitionCond_Chase_Sweep;
        transitionChaseSweep.notify = TransitionNotify_Chase_Sweep;
        stateChase.transitions[3] = transitionChaseSweep;

        Transition transitionLeapChase = new Transition();
        transitionLeapChase.from = stateLeap;
        transitionLeapChase.to = stateChase;
        transitionLeapChase.cond = TransitionCond_Leap_Chase;
        stateLeap.transitions[0] = transitionLeapChase;

        Transition transitionShotsShotsShotsChase = new Transition();
        transitionShotsShotsShotsChase.from = stateShotsShotsShots;
        transitionShotsShotsShotsChase.to = stateChase;
        transitionShotsShotsShotsChase.cond = TransitionCond_ShotsShotsShots_Chase;
        stateShotsShotsShots.transitions[0] = transitionShotsShotsShotsChase;

        Transition transitionChargeChase = new Transition();
        transitionChargeChase.from = stateCharge;
        transitionChargeChase.to = stateChase;
        transitionChargeChase.cond = TransitionCond_Charge_Chase;
        stateCharge.transitions[0] = transitionChargeChase;

        Transition transitionSweepChase = new Transition();
        transitionSweepChase.from = stateSweep;
        transitionSweepChase.to = stateChase;
        transitionSweepChase.cond = TransitionCond_Sweep_Chase;
        stateSweep.transitions[0] = transitionSweepChase;


    }

    public StateID state {
        get {
            return currentState.id;
        }

        set {
            foreach (var s in states) {
                if (s.id == value) {
                    TransitionTo(s);
                    return;
                }
            }
        }
    }

    public override string stateName {
        get {
            return state.ToString();
        }

        set {
            try {
                state = (StateID)System.Enum.Parse(typeof(StateID), value);
            } catch (System.ArgumentException) {
                throw new System.ArgumentException("Given string is not a valid state name!");
            }
        }
    }

    public override void Initialize(GameObject obj) {
        base.Initialize(obj);

        CallIfSet(currentState.enter);
    }

    public override void Update() {
        CallIfSet(currentState.during);

        State cur = currentState;

        for (int i = 0; i < maxTransitionsPerUpdate; i++) {
            bool found = false;

            foreach (var t in cur.transitions) {
                if (t.cond()) {
                    CallIfSet(t.notify);
                    cur = t.to;
                    found = true;
                }
            }

            if (!found) {
                break;
            }
        }

        if (cur != currentState) {
            TransitionTo(cur);
        }
    }

    private void TransitionTo(State target) {
        CallIfSet(currentState.exit);
        currentState = target;
        CallIfSet(target.enter);
    }

    protected virtual void StateEnter_Chase() { }
    protected virtual void State_Chase() { }
    protected virtual void StateEnter_Leap() { }
    protected virtual void StateEnter_ShotsShotsShots() { }
    protected virtual void StateEnter_Charge() { }
    protected virtual void StateEnter_Sweep() { }

    protected virtual bool TransitionCond_Chase_Leap() { return false; }
    protected virtual bool TransitionCond_Chase_ShotsShotsShots() { return false; }
    protected virtual bool TransitionCond_Chase_Charge() { return false; }
    protected virtual bool TransitionCond_Chase_Sweep() { return false; }
    protected virtual void TransitionNotify_Chase_Sweep() { }
    protected virtual bool TransitionCond_Leap_Chase() { return false; }
    protected virtual bool TransitionCond_ShotsShotsShots_Chase() { return false; }
    protected virtual bool TransitionCond_Charge_Chase() { return false; }
    protected virtual bool TransitionCond_Sweep_Chase() { return false; }

}
