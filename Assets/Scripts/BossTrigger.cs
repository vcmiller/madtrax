﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;
using System;

public class BossTrigger : MonoBehaviour {
    public Health bossHealth;

    public Healthbar bossHealthbar;
    public WinScreen winScreen;

    public GameObject wall;

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

            if (wall) {
                wall.SetActive(true);
            }
        }
    }

    private void Update() {
        if (!bossHealth && wall) {
            wall.SetActive(false);
        }
    }
}