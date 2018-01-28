using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatThrown : MonoBehaviour {

    Vector3 direction;
    public float toKill;
    public float speed;
	// Use this for initialization
	void Start () {
        Invoke("Die", toKill);

        direction = Vector3.ProjectOnPlane(FindObjectOfType<PlayerDamage>().transform.position - transform.position, Vector3.up).normalized;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(speed * Time.deltaTime * direction, Space.World);
	}

    public void Die()
    {
        Destroy(gameObject);
    }
}
