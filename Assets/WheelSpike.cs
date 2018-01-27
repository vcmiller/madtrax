using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpike : MonoBehaviour {

    public DriftRotator driftRotator;
    public float maxRotationSpeed;
    public float rotationAcceleration;
    float rotationSpeed = 0;

    // Update is called once per frame
    void Update()
    {
        float targetRotationSpeed = maxRotationSpeed * (driftRotator.motor.velocity.magnitude / driftRotator.motor.walkSpeed);
        rotationSpeed = Mathf.MoveTowards(rotationSpeed, targetRotationSpeed, Time.deltaTime * rotationAcceleration);

        transform.Rotate(Vector3.right * Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        BossHealth bh;
        if (bh = other.GetComponent<BossHealth>())
        {
            bh.Damage(10);
        }
    }
}
