using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SBR {
    [CreateAssetMenu(menuName = "State Machine Definition")]
    public class StateMachineDefinition : ScriptableObject {
        [Serializable]
        public class State {
            public string name;

            public bool hasEnter = false;
            public bool hasDuring = true;
            public bool hasExit = false;

            public List<Transition> transitions;

            [HideInInspector]
            public Vector2 position;

            public Vector2 size {
                get {
                    return new Vector2(192, 48);
                }
            }

            public Rect rect {
                get {
                    return new Rect(position, size);
                }
            }

            public Vector2 center {
                get {
                    return position + size / 2;
                }
            }

            public Transition GetTransition(State target) {
                if (transitions == null) {
                    return null;
                }

                return transitions.Find((t) => {
                    return t.to == target.name;
                });
            }

            public void RemoveTransition(Transition tr) {
                transitions.Remove(tr);
            }

            public void RemoveTransition(State target) {
                if (transitions != null) {
                    transitions.RemoveAll((t) => {
                        return t.to == target.name;
                    });
                }
            }

            public Transition AddTransition(State target) {
                if (target != this && GetTransition(target) == null) {
                    Transition newT = new Transition();
                    newT.to = target.name;

                    if (transitions == null) {
                        transitions = new List<Transition>();
                    }

                    transitions.Add(newT);
                    return newT;
                } else {
                    return null;
                }
            }
        }

        [Serializable]
        public class Transition {
            public string to;
            public bool hasNotify = false;

            public int width {
                get {
                    return 5;
                }
            }
        }

        public string defaultState;
        public List<State> states;
        public string baseClass;

        public State GetState(string name) {
            foreach (State def in states) {
                if (def.name == name) {
                    return def;
                }
            }

            return null;
        }

        public State SelectState(Vector2 position) {
            if (states != null) {
                foreach (State def in states) {
                    if (def.rect.Contains(position)) {
                        return def;
                    }
                }
            }

            return null;
        }

        public Pair<Vector2, Vector2> GetTransitionPoints(State from, Transition t) {
            State to = GetState(t.to);

            Vector2 dest = to.center;
            Vector2 src = from.center;

            Vector2 v = dest - src;
            v = v.normalized;
            Vector2 ortho = new Vector2(v.y, -v.x);

            src += ortho * t.width;
            dest += ortho * t.width;

            return new Pair<Vector2, Vector2>(src, dest);
        }

        public Pair<State, Transition> SelectTransition(Vector2 position) {
            foreach (State from in states) {
                if (from.transitions != null) {
                    foreach (Transition tr in from.transitions) {
                        if (GetState(tr.to) == null) {
                            continue;
                        }

                        var line = GetTransitionPoints(from, tr);

                        Vector2 src = line.t1;
                        Vector2 dest = line.t2;

                        Vector2 v = (dest - src).normalized;

                        float angle = Mathf.Atan2(v.y, v.x);

                        Vector2 pr = position - src;

                        Quaternion q = Quaternion.Euler(0, 0, -angle * Mathf.Rad2Deg);

                        pr = q * pr;

                        Rect rect = new Rect(0, -tr.width * 1.5f, Vector2.Distance(src, dest), tr.width * 3);

                        if (rect.Contains(pr)) {
                            return new Pair<State, Transition>(from, tr);
                        }
                    }
                }
            }

            return new Pair<State, Transition>(null, null);
        }

        public State AddState() {
            State newState = new State();
            states.Add(newState);
            return newState;
        }

        public void RemoveState(State toRemove) {
            foreach (State s in states) {
                s.RemoveTransition(toRemove);
            }

            states.Remove(toRemove);
        }

        public void RenameState(State toRename, string newName) {
            foreach (State s in states) {
                var t = s.GetTransition(toRename);
                if (t != null) {
                    t.to = newName;
                }
            }

            toRename.name = newName;
        }
    }
}