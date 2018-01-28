using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class WheelSpike : MonoBehaviour {

    public DriftMotor driftRotator;
    public float maxRotationSpeed;
    public float rotationAcceleration;
    float rotationSpeed = 0;
    public float damage = 5;

    // Update is called once per frame
    void Update() {
        float targetRotationSpeed = maxRotationSpeed * (driftRotator.motor.velocity.magnitude / driftRotator.motor.walkSpeed);
        rotationSpeed = Mathf.MoveTowards(rotationSpeed, targetRotationSpeed, Time.deltaTime * rotationAcceleration);

        if (!Input.GetButton("Fire2")) {
            transform.Rotate(Vector3.right * Time.deltaTime * rotationSpeed);
        }
    }

    private void OnTriggerEnter(Collider other) {
        Health bh;
        if ((bh = other.GetComponent<Health>()) && !Input.GetButton("Fire2")) {
            bh.ApplyDamage(new Damage(damage * rotationSpeed / maxRotationSpeed, transform.position, transform.forward));
        }
    }
}
