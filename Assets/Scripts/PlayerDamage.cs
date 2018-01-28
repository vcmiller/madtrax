using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class PlayerDamage : BasicMotor<FrancineChannels> {
    public float redFlashTime = 0.2f;
    public float killY = -10;

    public float damageDisableTime = 1;
    public float upHeight = 4;
    public float airMovement = 5;

    public Collider hitbox;
    public Motor[] affectedMotors;
    public Transform flippydoo;
    public Vector3 fdCenter = Vector3.up * 2;

    private bool damaged = false;
    private Vector3 rotAxis;
    private Vector3 startPos;
    private Quaternion startRot;
    private CharacterMotor characterMotor;

    private ExpirationTimer flashTimer;
    private ExpirationTimer penaltyTimer;

    public AudioSource damageSound;

    protected override void Start() {
        base.Start();

        flashTimer = new ExpirationTimer(redFlashTime);
        penaltyTimer = new ExpirationTimer(damageDisableTime);
        characterMotor = GetComponentInChildren<CharacterMotor>();
    }

    void DamageNotify(Damage dmg) {
        flashTimer.Set();

        if (dmg.amount < 100) {
            damageSound.Play();

            penaltyTimer.Set();
            damaged = true;

            startPos = flippydoo.localPosition;
            startRot = flippydoo.localRotation;
            rotAxis = Random.insideUnitCircle.normalized;
            rotAxis.z = rotAxis.y;
            rotAxis.y = 0;
            
            foreach (var mot in affectedMotors) {
                mot.enableInput = false;
            }
        }
    }

    void ZeroHealth() {
        GetComponent<Brain>().enabled = false;
    }

    public override void TakeInput() {
        if (transform.position.y < killY) {
            GetComponent<Health>().ApplyDamage(new Damage(10000, transform.position, Vector3.up));
        }

        if (!penaltyTimer.expired) {
            hitbox.enabled = false;

            characterMotor.velocity = channels.movement2 * airMovement;

            float f = 1 - penaltyTimer.remainingRatio;
            float s = 2 * f - 1;
            float h = 1 - s * s;

            Quaternion r = Quaternion.AngleAxis(360 * f, rotAxis);
            flippydoo.localPosition = startPos + Vector3.up * upHeight * h + fdCenter - r * fdCenter;
            flippydoo.localRotation = r * startRot;

        } else if (damaged) {
            flippydoo.localPosition = startPos;
            flippydoo.localRotation = startRot;
            
            damaged = false;
            hitbox.enabled = true;
            
            foreach (var mot in affectedMotors) {
                mot.enableInput = true;
            }
        }
    }

    private void OnGUI() {
        if (!flashTimer.expired) {
            GUI.color = new Color(0.7f, 0.0f, 0.0f, 0.3f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }
}
