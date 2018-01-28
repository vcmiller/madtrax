using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {
    Transform player;
    bool playedMusic;
    public AudioClip clip;
    public UnityEngine.UI.Text textcredit;
 
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerDamage>().transform;
	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(player.position, transform.position) < 100)
        {
            if (!playedMusic)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
                playedMusic = true;

                textcredit.color = Color.white;

                Destroy(player.GetComponent<SBR.Brain>());
            }
            player.parent = transform;
            player.localPosition = Vector3.Lerp(player.localPosition, Vector3.zero, 0.25f);
            player.localRotation = Quaternion.Lerp(player.localRotation, Quaternion.Euler(90,0,0), 0.25f);
        }
	}
}
