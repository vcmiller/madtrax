using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorInterface : MonoBehaviour {

    private MohawkBossImpl mhbImpl;
    private bool isCharging;
    private bool isLeaping;
    private bool isTracking;

    public float trackTime;

    public float chargeSpeed;
    public float chargeTime;

    public float leapHangtime;
    private float leapSpeed;

    // Use this for initialization
    void Start () {
        mhbImpl = transform.root.GetComponent<SBR.Brain>().activeController as MohawkBossImpl;
	}

    private void Update()
    {
        print(mhbImpl.transform.forward);
        if (isCharging)
        {
            mhbImpl.transform.Translate(mhbImpl.transform.forward * Time.deltaTime * chargeSpeed, Space.World);
        }else if (isLeaping)
        {
            mhbImpl.transform.Translate(mhbImpl.transform.forward * Time.deltaTime * leapSpeed, Space.World);
        }
    }

    public void NotifyEndAnim()
    {
        mhbImpl.endAnimationFlag = true;
    }

    public void SetTracking(int isTracking)
    {
        if(isTracking == 1)
        {
            Invoke("StopTracking", trackTime);
        }
        this.isTracking = isTracking == 1;
    }

    public void StopTracking()
    {
        SetTracking(0);
    }

    public void SetCharging(int isCharging)
    {
        if(isCharging == 1)
        {
            Invoke("StopCharging", chargeTime);
        }
        this.isCharging = isCharging == 1;
    }

    public void StopCharging()
    {
        SetCharging(0);
    }


    public void SetLeaping(int isLeaping)
    {
        if (isLeaping == 1)
        {
            Vector3 distanceToPlayer = Vector3.ProjectOnPlane(mhbImpl.player.transform.position - mhbImpl.transform.position, Vector3.up);
            leapSpeed = (distanceToPlayer.magnitude - 2)/ leapHangtime;
            Invoke("StopLeaping", leapHangtime);
        }
        this.isLeaping = isLeaping == 1;
    }

    public void StopLeaping() {
        SetLeaping(0);
    }

}
