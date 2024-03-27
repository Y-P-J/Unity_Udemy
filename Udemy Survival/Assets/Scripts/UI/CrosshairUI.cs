using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("ÄÄÆ÷³ÍÆ®")]
    GameObject crosshairUI;
    Animator myAnim;



    void Start()
    {
        crosshairUI = GetComponent<GameObject>();
        myAnim = GetComponent<Animator>();
    }

    public void SetAnimPlayerParameters(bool _isWalk, bool _isRun, bool _isCrouch)
    {
        myAnim.SetBool("isWalk", _isWalk);
        myAnim.SetBool("isRun", _isRun);
        myAnim.SetBool("isCrouch", _isCrouch);
    }

    public void SetAnimGunParameters(bool _isADS)
    {
        myAnim.SetBool("isADS", _isADS);
    }

    public void SetAnimFireParameters()
    {
        if (myAnim.GetBool("isWalk"))
            myAnim.SetTrigger("onWalkFire");
        else if (myAnim.GetBool("isCrouch"))
            myAnim.SetTrigger("onCrouchFire");
        else
            myAnim.SetTrigger("onIdleFire");
    }
}
