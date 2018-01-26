using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SBR {
    public class MoveStateOperation : Operation {
        public static int snap = 16;

        private Vector2 start;
        private Vector2 move;

        public MoveStateOperation(StateMachineDefinition def, StateMachineDefinition.State state) : base(def, state) {
            showBaseGUI = true;
            start = state.position;
            move = Vector2.zero;
        }

        public override void Update() {
            var evt = Event.current;
            if (evt.type == EventType.MouseUp && evt.button == 0) {
                done = true;
            } else if (evt.type == EventType.MouseDrag && evt.button == 0) {
                move += evt.delta;
                state.position = start + move;
                Snap(ref state.position);
                repaint = true;
            }

        }

        public override void Cancel() {
            state.position = start;
        }

        public override void Confirm() {
        }

        public override void OnGUI() {
        }

        private void Snap(ref Vector2 input) {
            input.x = Mathf.Round(input.x / snap) * snap;
            input.y = Mathf.Round(input.y / snap) * snap;
        }
    }
}