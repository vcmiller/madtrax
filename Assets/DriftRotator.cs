using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftRotator : MonoBehaviour {

    public Transform aestheticTarget;
    public float turnSpeed = 15;
    public float maxTilt = 80f;
    CharacterMotor motor;
    Brain brain;

    public DriftController drift
    {
        get
        {
            return brain.activeController as DriftController;
        }
    }
    
	// Use this for initialization
	void Start () {
        brain = GetComponent<Brain>();
        motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 will = drift.rightInput * Vector3.right + drift.fwdInput * Vector3.forward;
        will = Vector3.ClampMagnitude(will, 1);

        float angle = will.magnitude > 0 ? Vector3.Cross(Quaternion.Euler(0, 0, 0) * will, aestheticTarget.forward).y : 0;

        Quaternion targetRotation = (will.magnitude > 0.01f) ? Quaternion.LookRotation(will, Vector3.up) : aestheticTarget.rotation;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, maxTilt * angle);

        aestheticTarget.rotation = Quaternion.RotateTowards(aestheticTarget.rotation, targetRotation, turnSpeed * Time.deltaTime);

        drift.character.movement += aestheticTarget.forward * will.magnitude ;
    }
}
