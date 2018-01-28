using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SBR;

public class Healthbar : MonoBehaviour {
    public Health health;

    public Slider slider { get; private set; }

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        slider.value = health.health / health.maxHealth;
	}
}
