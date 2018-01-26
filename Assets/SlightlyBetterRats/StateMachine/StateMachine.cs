using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public abstract class StateMachine : Controller {
        public int maxTransitionsPerUpdate { get; private set; }

        protected delegate void Notify();
        protected delegate bool Condition();

        public StateMachine() {
            maxTransitionsPerUpdate = 16;
        }

        public abstract string stateName {
            get; set;
        }

        protected void CallIfSet(Notify notify) {
            if (notify != null) {
                notify();
            }
        }
    }
}