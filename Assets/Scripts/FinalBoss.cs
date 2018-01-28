using UnityEngine;
using SBR;

public abstract class FinalBoss : SBR.StateMachine {
    public enum StateID {
        idle, Rising, ThrowingBoats, Charge, BoatSprout, Chasing
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

    public FinalBoss() {
        states = new State[6];

        State stateidle = new State();
        stateidle.id = StateID.idle;
        stateidle.transitions = new Transition[1];
        states[0] = stateidle;

        State stateRising = new State();
        stateRising.id = StateID.Rising;
        stateRising.during = State_Rising;
        stateRising.transitions = new Transition[1];
        states[1] = stateRising;

        State stateThrowingBoats = new State();
        stateThrowingBoats.id = StateID.ThrowingBoats;
        stateThrowingBoats.during = State_ThrowingBoats;
        stateThrowingBoats.exit = StateExit_ThrowingBoats;
        stateThrowingBoats.transitions = new Transition[1];
        states[2] = stateThrowingBoats;

        State stateCharge = new State();
        stateCharge.id = StateID.Charge;
        stateCharge.during = State_Charge;
        stateCharge.transitions = new Transition[1];
        states[3] = stateCharge;

        State stateBoatSprout = new State();
        stateBoatSprout.id = StateID.BoatSprout;
        stateBoatSprout.enter = StateEnter_BoatSprout;
        stateBoatSprout.transitions = new Transition[1];
        states[4] = stateBoatSprout;

        State stateChasing = new State();
        stateChasing.id = StateID.Chasing;
        stateChasing.during = State_Chasing;
        stateChasing.transitions = new Transition[3];
        states[5] = stateChasing;

        currentState = stateidle;

        Transition transitionidleRising = new Transition();
        transitionidleRising.from = stateidle;
        transitionidleRising.to = stateRising;
        transitionidleRising.cond = TransitionCond_idle_Rising;
        stateidle.transitions[0] = transitionidleRising;

        Transition transitionRisingChasing = new Transition();
        transitionRisingChasing.from = stateRising;
        transitionRisingChasing.to = stateChasing;
        transitionRisingChasing.cond = TransitionCond_Rising_Chasing;
        stateRising.transitions[0] = transitionRisingChasing;

        Transition transitionThrowingBoatsChasing = new Transition();
        transitionThrowingBoatsChasing.from = stateThrowingBoats;
        transitionThrowingBoatsChasing.to = stateChasing;
        transitionThrowingBoatsChasing.cond = TransitionCond_ThrowingBoats_Chasing;
        stateThrowingBoats.transitions[0] = transitionThrowingBoatsChasing;

        Transition transitionChargeChasing = new Transition();
        transitionChargeChasing.from = stateCharge;
        transitionChargeChasing.to = stateChasing;
        transitionChargeChasing.cond = TransitionCond_Charge_Chasing;
        stateCharge.transitions[0] = transitionChargeChasing;

        Transition transitionBoatSproutChasing = new Transition();
        transitionBoatSproutChasing.from = stateBoatSprout;
        transitionBoatSproutChasing.to = stateChasing;
        transitionBoatSproutChasing.cond = TransitionCond_BoatSprout_Chasing;
        stateBoatSprout.transitions[0] = transitionBoatSproutChasing;

        Transition transitionChasingThrowingBoats = new Transition();
        transitionChasingThrowingBoats.from = stateChasing;
        transitionChasingThrowingBoats.to = stateThrowingBoats;
        transitionChasingThrowingBoats.cond = TransitionCond_Chasing_ThrowingBoats;
        stateChasing.transitions[0] = transitionChasingThrowingBoats;

        Transition transitionChasingBoatSprout = new Transition();
        transitionChasingBoatSprout.from = stateChasing;
        transitionChasingBoatSprout.to = stateBoatSprout;
        transitionChasingBoatSprout.cond = TransitionCond_Chasing_BoatSprout;
        stateChasing.transitions[1] = transitionChasingBoatSprout;

        Transition transitionChasingCharge = new Transition();
        transitionChasingCharge.from = stateChasing;
        transitionChasingCharge.to = stateCharge;
        transitionChasingCharge.cond = TransitionCond_Chasing_Charge;
        stateChasing.transitions[2] = transitionChasingCharge;


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

    protected virtual void State_Rising() { }
    protected virtual void State_ThrowingBoats() { }
    protected virtual void StateExit_ThrowingBoats() { }
    protected virtual void State_Charge() { }
    protected virtual void StateEnter_BoatSprout() { }
    protected virtual void State_Chasing() { }

    protected virtual bool TransitionCond_idle_Rising() { return false; }
    protected virtual bool TransitionCond_Rising_Chasing() { return false; }
    protected virtual bool TransitionCond_ThrowingBoats_Chasing() { return false; }
    protected virtual bool TransitionCond_Charge_Chasing() { return false; }
    protected virtual bool TransitionCond_BoatSprout_Chasing() { return false; }
    protected virtual bool TransitionCond_Chasing_ThrowingBoats() { return false; }
    protected virtual bool TransitionCond_Chasing_BoatSprout() { return false; }
    protected virtual bool TransitionCond_Chasing_Charge() { return false; }

}
