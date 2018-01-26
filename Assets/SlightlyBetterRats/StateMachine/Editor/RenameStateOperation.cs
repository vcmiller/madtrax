using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SBR {
    public class RenameStateOperation : Operation {
        private string name;

        public RenameStateOperation(StateMachineDefinition def, StateMachineDefinition.State state) : base(def, state) {
            name = state.name;
            showBaseGUI = false;
        }

        public override void Update() {
            var evt = Event.current;
            if (evt.type == EventType.KeyDown) {
                if (evt.keyCode == KeyCode.Return) {
                    Confirm();
                    done = true;
                } else if (evt.keyCode == KeyCode.Escape) {
                    Cancel();
                    done = true;
                }
            } else if (evt.type == EventType.MouseDown) {
                if (definition.SelectState(evt.mousePosition) != state) {
                    Confirm();
                    done = true;
                }
            }
        }

        public override void Cancel() {
        }

        public override void Confirm() {
            definition.RenameState(state, name);
        }

        public override void OnGUI() {
            name = EditorGUI.TextField(state.rect, name);
        }
    }
}
