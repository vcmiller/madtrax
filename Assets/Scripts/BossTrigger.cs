using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;
using System;

public class BossTrigger : MonoBehaviour {

    public static HashSet<BossTrigger> bosses = new HashSet<BossTrigger>();

    public Health bossHealth;

    public Healthbar bossHealthbar;
    public WinScreen winScreen;

    public GameObject wall;

    public AudioClip bossTrack;

    public Cinemachine.CinemachineTargetGroup group;
 

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && bossHealth) {
            Destroy(GetComponent<Collider>());
            bossHealth.GetComponent<Brain>().enabled = true;

            bossHealthbar.health = bossHealth;
            winScreen.target = bossHealth.gameObject;

            Array.Resize(ref group.m_Targets, group.m_Targets.Length + 1);

            var t = new Cinemachine.CinemachineTargetGroup.Target();
            t.target = bossHealth.transform;
            t.weight = 1;
            t.radius = 3;

            group.m_Targets[group.m_Targets.Length - 1] = t;

            if (bossTrack) {
                MusicPlayer.inst.ChangeTrack(bossTrack);
            }

            if (wall) {
                wall.SetActive(true);
            }
        }
    }

    private void Update() {
        if (!bossHealth && wall) {
            if (!bosses.Contains(this))
            {
                bosses.Add(this);
                if (FindObjectsOfType<MohawkBossDamage>().Length == 0)
                {
                    FindObjectOfType<PlayerDamage>().killY = -550;
                    Destroy(GameObject.Find("MapGeometry"));
                    Vector3 playerpos = FindObjectOfType<PlayerDamage>().transform.position;
                    FindObjectOfType<Boat>().transform.position = new Vector3(playerpos.x, FindObjectOfType<Boat>().transform.position.y, playerpos.z);
                }
            }

            if (Input.GetKey(KeyCode.Backspace))
            {
                foreach(MohawkBossDamage mbd in FindObjectsOfType<MohawkBossDamage>())
                {
                    Destroy(mbd.gameObject);
                }
                FindObjectOfType<PlayerDamage>().killY = -550;
                GameObject go = GameObject.Find("MapGeometry");
                if (go)
                {
                    Destroy(go);
                }
                Vector3 playerpos = FindObjectOfType<PlayerDamage>().transform.position;
                FindObjectOfType<Boat>().transform.position = new Vector3(playerpos.x, FindObjectOfType<Boat>().transform.position.y, playerpos.z);
            }

            MusicPlayer.inst.ChangeToDefault();

            wall.SetActive(false);
        }
    }
}
