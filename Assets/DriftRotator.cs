using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftRotator : MonoBehaviour {

    public Transform aestheticTarget;
    public Transform sphere;
    public float turnSpeed = 15;

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

        float angle = Vector3.Cross(Quaternion.Euler(0, 45, 0) * will, aestheticTarget.forward).y;

        Quaternion targetRotation = (motor.velocity.magnitude > 0.01f) ? Quaternion.LookRotation(motor.velocity, Vector3.up) : aestheticTarget.rotation;
        aestheticTarget.rotation = Quaternion.RotateTowards(aestheticTarget.rotation, targetRotation, turnSpeed * Time.deltaTime);
        targetRotation *= Quaternion.Euler(Vector3.forward * 40 * angle);
        aestheticTarget.rotation = Quaternion.RotateTowards(aestheticTarget.rotation, targetRotation, turnSpeed * Time.deltaTime);


        drift.character.movement += will.magnitude * aestheticTarget.forward;

        print(Quaternion.Euler(0,45,0) * will +"::"+ aestheticTarget.forward);
        //sphere.position = aestheticTarget.position + ;
    }
}
