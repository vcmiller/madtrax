using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public class BasicCharacterController : PlayerController {
        public CharacterChannels character { get; private set; }

        public override void Initialize(GameObject obj) {
            base.Initialize(obj);

            character = channels as CharacterChannels;
        }


        public void Axis_Horizontal(float value) {
            Vector3 right = viewTarget.transform.right;
            right.y = 0;
            right = right.normalized;

            character.movement += right * value;
        }

        public void Axis_Vertical(float value) {
            Vector3 fwd = viewTarget.transform.forward;
            fwd.y = 0;
            fwd = fwd.normalized;

            character.movement += fwd * value;
        }

        public void ButtonDown_Jump() {
            character.jump = true;
        }

        public void BUttonUp_Jump() {
            character.jump = false;
        }

        public void Axis_MouseX(float value) {
            character.rotation = Quaternion.Euler(0, value, 0) * character.rotation;
        }

        public void Axis_MouseY(float value) {
            character.rotation *= Quaternion.Euler(-value, 0, 0);
        }
    }
}