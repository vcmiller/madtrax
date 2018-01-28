using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakSomeStuff : MonoBehaviour {
    public GameObject[] stuffThatBreak;

	// Use this for initialization
	void Start () {
		foreach (var obj in stuffThatBreak) {
            Destroy(obj);
        }
	}
}
