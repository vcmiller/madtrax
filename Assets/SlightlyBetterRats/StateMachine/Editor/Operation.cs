using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SBR {
    public abstract class Operation {
        public bool _repaint;

        public bool done { get; protected set; }
        public bool repaint {
            get {
                bool b = _repaint;
                _repaint = false;
                return b;
            }

            protected set {
                _repaint = value;
            }
        }
        public StateMachineDefinition definition { get; private set; }
        public StateMachineDefinition.State state { get; private set; }
        public bool showBaseGUI { get; protected set; }

        public Operation(StateMachineDefinition definition, StateMachineDefinition.State state) {
            this.definition = definition;
            this.state = state;
        }

        public abstract void Update();
        public abstract void Cancel();
        public abstract void Confirm();
        public abstract void OnGUI();
    }
}
