using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftController : PlayerController {

    public FrancineChannels character { get; private set; }

    public override void Initialize(GameObject obj) {
        base.Initialize(obj);

        character = channels as FrancineChannels;
    }

    public void Axis_Horizontal(float value) {
        character.movement2 += Camera.main.transform.right * value;
    }

    public void Axis_Vertical(float value) {
        Vector3 v = Camera.main.transform.forward;
        v.y = 0;
        v = v.normalized;

        character.movement2 += v * value;
    }

    public void Axis_ShootHorizontal(float value) {
        character.aim += Camera.main.transform.right * value;
    }

    public void Axis_ShootVertical(float value) {
        Vector3 v = Camera.main.transform.forward;
        v.y = 0;
        v = v.normalized;

        character.aim -= v * value;
    }
}
