using UnityEngine;
using SBR;

public abstract class TestSM : SBR.StateMachine {
    public enum StateID {
        Patrol, Idle, Pursue
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

    public TestSM() {
        states = new State[3];

        State statePatrol = new State();
        statePatrol.id = StateID.Patrol;
        statePatrol.enter = StateEnter_Patrol;
        statePatrol.during = State_Patrol;
        statePatrol.exit = StateExit_Patrol;
        statePatrol.transitions = new Transition[2];
        states[0] = statePatrol;

        State stateIdle = new State();
        stateIdle.id = StateID.Idle;
        stateIdle.enter = StateEnter_Idle;
        stateIdle.during = State_Idle;
        stateIdle.exit = StateExit_Idle;
        stateIdle.transitions = new Transition[1];
        states[1] = stateIdle;

        State statePursue = new State();
        statePursue.id = StateID.Pursue;
        statePursue.enter = StateEnter_Pursue;
        statePursue.during = State_Pursue;
        statePursue.exit = StateExit_Pursue;
        statePursue.transitions = new Transition[1];
        states[2] = statePursue;

        currentState = statePatrol;

        Transition transitionPatrolIdle = new Transition();
        transitionPatrolIdle.from = statePatrol;
        transitionPatrolIdle.to = stateIdle;
        transitionPatrolIdle.cond = TransitionCond_Patrol_Idle;
        transitionPatrolIdle.notify = TransitionNotify_Patrol_Idle;
        statePatrol.transitions[0] = transitionPatrolIdle;

        Transition transitionPatrolPursue = new Transition();
        transitionPatrolPursue.from = statePatrol;
        transitionPatrolPursue.to = statePursue;
        transitionPatrolPursue.cond = TransitionCond_Patrol_Pursue;
        transitionPatrolPursue.notify = TransitionNotify_Patrol_Pursue;
        statePatrol.transitions[1] = transitionPatrolPursue;

        Transition transitionIdlePatrol = new Transition();
        transitionIdlePatrol.from = stateIdle;
        transitionIdlePatrol.to = statePatrol;
        transitionIdlePatrol.cond = TransitionCond_Idle_Patrol;
        transitionIdlePatrol.notify = TransitionNotify_Idle_Patrol;
        stateIdle.transitions[0] = transitionIdlePatrol;

        Transition transitionPursuePatrol = new Transition();
        transitionPursuePatrol.from = statePursue;
        transitionPursuePatrol.to = statePatrol;
        transitionPursuePatrol.cond = TransitionCond_Pursue_Patrol;
        transitionPursuePatrol.notify = TransitionNotify_Pursue_Patrol;
        statePursue.transitions[0] = transitionPursuePatrol;


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
        currentState.during();

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

    protected virtual void StateEnter_Patrol() { }
    protected virtual void State_Patrol() { }
    protected virtual void StateExit_Patrol() { }
    protected virtual void StateEnter_Idle() { }
    protected virtual void State_Idle() { }
    protected virtual void StateExit_Idle() { }
    protected virtual void StateEnter_Pursue() { }
    protected virtual void State_Pursue() { }
    protected virtual void StateExit_Pursue() { }

    protected virtual bool TransitionCond_Patrol_Idle() { return false; }
    protected virtual void TransitionNotify_Patrol_Idle() { }
    protected virtual bool TransitionCond_Patrol_Pursue() { return false; }
    protected virtual void TransitionNotify_Patrol_Pursue() { }
    protected virtual bool TransitionCond_Idle_Patrol() { return false; }
    protected virtual void TransitionNotify_Idle_Patrol() { }
    protected virtual bool TransitionCond_Pursue_Patrol() { return false; }
    protected virtual void TransitionNotify_Pursue_Patrol() { }

}
