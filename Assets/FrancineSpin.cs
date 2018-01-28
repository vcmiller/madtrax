using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrancineSpin : MonoBehaviour {

    public SBR.Health health;
    	
	// Update is called once per frame
	void Update () {
	    if(health.health <= 0)
        {
            transform.rotation = Quaternion.Euler(Random.onUnitSphere);
        }	
	}
}
