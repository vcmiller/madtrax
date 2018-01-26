using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public class Health : MonoBehaviour {
        public float health { get; private set; }
        public float maxHealth;

        public bool repeatZeroHealth;

        public bool hasDied { get; private set; }

        // Use this for initialization
        protected virtual void Start() {
            health = maxHealth;
        }

        public virtual void ApplyDamage(Damage dmg) {
            if (enabled) {
                health -= dmg.amount;
                health = Mathf.Max(health, 0);
                SendMessage("DamageNotify", dmg, SendMessageOptions.DontRequireReceiver);

                if (health == 0 && (!hasDied || repeatZeroHealth)) {
                    hasDied = true;
                    SendMessage("ZeroHealth", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
