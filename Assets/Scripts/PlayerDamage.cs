using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class PlayerDamage : MonoBehaviour {
    public float redFlashTime = 0.2f;

    private ExpirationTimer flashTimer;

    private void Start() {
        flashTimer = new ExpirationTimer(redFlashTime);
    }

    void DamageNotify(Damage dmg) {
        flashTimer.Set();
    }

    private void OnGUI() {
        if (!flashTimer.expired) {
            GUI.color = new Color(0.7f, 0.0f, 0.0f, 0.3f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }
}
