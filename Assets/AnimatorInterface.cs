using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorInterface : MonoBehaviour {

    public MohawkBossImpl mhbImpl;

	// Use this for initialization
	void Start () {
        mhbImpl = GetComponent<SBR.Brain>().activeController as MohawkBossImpl;
	}
	
    public void NotifyEndAnim()
    {
        mhbImpl.endAnimationFlag = true;
    }
}
