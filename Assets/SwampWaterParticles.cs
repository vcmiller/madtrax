using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampWaterParticles : MonoBehaviour {

    public float waterY = -495f;
    public ParticleSystem sys;
    public DriftMotor drif;

	// Use this for initialization
	void Start () {
        sys.Stop();
	}
	
	// Update is called once per frame
	void Update () {
        if(!sys.isPlaying && drif.transform.position.y <= waterY)
            sys.Play();

        if (sys.isPlaying)
        {
            sys.emissionRate = 100 * (drif.motor.velocity.magnitude / drif.motor.walkSpeed);
        }
	}
}
