using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftController : PlayerController{

    public CharacterChannels character { get; private set; }
    public float rightInput = 0;
    public float fwdInput = 0;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        character = channels as CharacterChannels;
    }

    public void Axis_Horizontal(float value)
    {
        Vector3 right = viewTarget.transform.right;
        right.y = 0;
        right = right.normalized;

        rightInput = value;

        character.movement += right * value;
    }

    public void Axis_Vertical(float value)
    {
        Vector3 fwd = viewTarget.transform.forward;
        fwd.y = 0;
        fwd = fwd.normalized;

        fwdInput = value;

        character.movement += fwd * value;
    }
}
