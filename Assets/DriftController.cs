using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class DriftController : PlayerController{

    public CharacterChannels character { get; private set; }
    public float rightInput  { get; private set; }
    public float fwdInput  { get; private set; }

    public float shootRightInput { get; private set; }
    public float shootFwdInput { get; private set; }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        character = channels as CharacterChannels;
    }

    public void Axis_Horizontal(float value)
    { 
        rightInput = value;
    }

    public void Axis_Vertical(float value)
    { 
        fwdInput = value;
    }

    public void Axis_ShootHorizontal(float value)
    {
        shootRightInput = value;
    }

    public void Axis_ShootVertical(float value)
    {
        shootFwdInput = -value;
    }
}
