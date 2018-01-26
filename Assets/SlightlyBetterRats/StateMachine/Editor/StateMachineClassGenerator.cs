using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace SBR {
    public static class StateMachineClassGenerator {
        private static string implClassTemplate = @"using UnityEngine;
using SBR;

public class {0} : {1} {{
{2}
}}
";

        private static string abstractClassTemplate = @"using UnityEngine;
using SBR;

public abstract class {0} : {5} {{
    public enum StateID {{
        {1}
    }}

    private class State {{
        public StateID id;

        public Notify enter;
        public Notify during;
        public Notify exit;

        public Transition[] transitions;
    }}

    private class Transition {{
        public State from;
        public State to;

        public Notify notify;
        public Condition cond;
    }}

    private State[] states;
    private State currentState;

    public {0}() {{
{2}
{3}
    }}

    public StateID state {{
        get {{
            return currentState.id;
        }}

        set {{
            foreach (var s in states) {{
                if (s.id == value) {{
                    TransitionTo(s);
                    return;
                }}
            }}
        }}
    }}

    public override string stateName {{
        get {{
            return state.ToString();
        }}

        set {{
            try {{
                state = (StateID)System.Enum.Parse(typeof(StateID), value);
            }} catch (System.ArgumentException) {{
                throw new System.ArgumentException(""Given string is not a valid state name!"");
            }}
        }}
    }}

    public override void Initialize(GameObject obj) {{
        base.Initialize(obj);

        CallIfSet(currentState.enter);
    }}

    public override void Update() {{
        currentState.during();

        State cur = currentState;

        for (int i = 0; i < maxTransitionsPerUpdate; i++) {{
            bool found = false;

            foreach (var t in cur.transitions) {{
                if (t.cond()) {{
                    CallIfSet(t.notify);
                    cur = t.to;
                    found = true;
                }}
            }}

            if (!found) {{
                break;
            }}
        }}

        if (cur != currentState) {{
            TransitionTo(cur);
        }}
    }}

    private void TransitionTo(State target) {{
        CallIfSet(currentState.exit);
        currentState = target;
        CallIfSet(target.enter);
    }}

{4}
}}
";

        public static void GenerateImplClass(StateMachineDefinition def, string path) {
            string className = Path.GetFileNameWithoutExtension(path);

            string generated = string.Format(implClassTemplate, className, def.name, GetFunctionDeclarations(def, true));

            StreamWriter outStream = new StreamWriter(path);
            outStream.Write(generated);
            outStream.Close();
            AssetDatabase.Refresh();
        }

        public static void GenerateAbstractClass(StateMachineDefinition def) {
            string generated = string.Format(abstractClassTemplate, def.name, GetStateEnums(def), GetStateInitializers(def), GetTransitionInitializers(def), GetFunctionDeclarations(def), def.baseClass);

            string defPath = AssetDatabase.GetAssetPath(def);

            if (defPath.Length > 0) {
                string newPath = defPath.Substring(0, defPath.LastIndexOf(".")) + ".cs";

                StreamWriter outStream = new StreamWriter(newPath);
                outStream.Write(generated);
                outStream.Close();
                AssetDatabase.Refresh();
            }
        }

        private static string GetStateEnums(StateMachineDefinition def) {
            string str = "";

            for (int i = 0; i < def.states.Count; i++) {
                str += def.states[i].name;

                if (i < def.states.Count - 1) {
                    str += ", ";
                }
            }

            return str;
        }

        public static string GetStateInitializers(StateMachineDefinition def) {
            string str = "        states = new State[" + def.states.Count + "];\n\n";
            for (int i = 0; i < def.states.Count; i++) {
                str += GetStateInitializer(def.states[i], i);
            }
            if (def.defaultState.Length > 0) {
                str += "        currentState = state" + def.defaultState + ";\n";
            } else {
                str += "        currentState = states[0];\n";
            }
            return str;
        }

        public static string GetStateInitializer(StateMachineDefinition.State state, int index) {
            string variable = "state" + state.name;

            string str = "        State " + variable + " = new State();\n";
            str += "        " + variable + ".id = StateID." + state.name + ";\n";

            if (state.hasEnter) {
                str += "        " + variable + ".enter = StateEnter_" + state.name + ";\n";
            }

            if (state.hasDuring) {
                str += "        " + variable + ".during = State_" + state.name + ";\n";
            }

            if (state.hasExit) {
                str += "        " + variable + ".exit = StateExit_" + state.name + ";\n";
            }
            str += "        " + variable + ".transitions = new Transition[" + state.transitions.Count + "];\n";
            str += "        states[" + index + "] = " + variable + ";\n";

            str += "\n";
            return str;
        }

        public static string GetTransitionInitializers(StateMachineDefinition def) {
            string str = "";
            foreach (var state in def.states) {
                for (int i = 0; i < state.transitions.Count; i++) {
                    str += GetTransitionInitializer(state, state.transitions[i], i);
                }
            }
            return str;
        }

        public static string GetTransitionInitializer(StateMachineDefinition.State from, StateMachineDefinition.Transition to, int index) {
            string variable = "transition" + from.name + to.to;

            string str = "        Transition " + variable + " = new Transition();\n";
            str += "        " + variable + ".from = state" + from.name + ";\n";
            str += "        " + variable + ".to = state" + to.to + ";\n";
            str += "        " + variable + ".cond = TransitionCond_" + from.name + "_" + to.to + ";\n";

            if (to.hasNotify) {
                str += "        " + variable + ".notify = TransitionNotify_" + from.name + "_" + to.to + ";\n";
            }

            str += "        state" + from.name + ".transitions[" + index + "] = " + variable + ";\n";

            str += "\n";
            return str;
        }

        public static string GetFunctionDeclarations(StateMachineDefinition def, bool over = false) {
            string vo = over ? "override" : "virtual";


            string str = "";
            foreach (var state in def.states) {
                if (state.hasEnter) {
                    str += "    protected " + vo + " void StateEnter_" + state.name + "() { }\n";
                }

                if (state.hasDuring) {
                    str += "    protected " + vo + " void State_" + state.name + "() { }\n";
                }

                if (state.hasExit) {
                    str += "    protected " + vo + " void StateExit_" + state.name + "() { }\n";
                }
            }

            str += "\n";

            foreach (var state in def.states) {
                foreach (var trans in state.transitions) {
                    str += "    protected " + vo + " bool TransitionCond_" + state.name + "_" + trans.to + "() { return false; }\n";

                    if (trans.hasNotify) {
                        str += "    protected " + vo + " void TransitionNotify_" + state.name + "_" + trans.to + "() { }\n";
                    }
                }
            }

            return str;
        }
    }
}