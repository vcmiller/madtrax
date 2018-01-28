using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class ToggleWalls : MonoBehaviour {
    public float cooldown;
    public GameObject[] walls;

    private CooldownTimer timer;

	// Use this for initialization
	void Start () {
        timer = new CooldownTimer(cooldown);
	}
	
	// Update is called once per frame
	void Update () {
		if (timer.Use()) {
            var wall = walls[Random.Range(0, walls.Length)];

            wall.SetActive(!wall.activeSelf);
        }
	}
}
