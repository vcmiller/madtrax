using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SBR;

public class Healthbar : MonoBehaviour {
    public Health health;

    public Slider slider { get; private set; }

    private bool isOn = true;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        if ((health != null) != isOn) {
            isOn = health;

            foreach (var img in GetComponentsInChildren<Image>()) {
                img.enabled = isOn;
            }
        }

        if (isOn) {
            slider.value = health.health / health.maxHealth;
        }
	}


}
