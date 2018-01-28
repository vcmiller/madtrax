using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialslide : MonoBehaviour {

    Renderer ren;

	// Use this for initialization
	void Start () {
        ren = GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        ren.sharedMaterial.SetTextureOffset("_MainTex", Vector2.right * Time.time);
	}
}
