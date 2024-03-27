using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeType
{
    FIST = 0,
    AXE = 1,
    PICKAXE = 2
}

public class Melee : MonoBehaviour
{

    [Header("컴포넌트")]
    Animator myAnim;

    

    [Header("수치 값")]
    [SerializeField] string meleeName;              //현재이름

    [SerializeField] int attackDamage;              //피해량
    [SerializeField] float attackRange;             //공격 사거리
    [SerializeField] float attackCooldown;          //공격 쿨타임

    [SerializeField] float workSpeed;               //작업 속도

    [SerializeField] float attackActivateDelay;     //공격 활성화 시점
    [SerializeField] float attackDeactivateDelay;   //공격 비활성화 시점

    [SerializeField] MeleeType meleeType;

    #region 람다식 호출문
    public Animator MyAnim => myAnim;

    public string MeleeName => meleeName;

    public float AttackRange => attackRange;

    public int AttackDamage => attackDamage;
    public float AttackActivateDelay => attackActivateDelay;
    public float AttackDeactivateDelay => attackDeactivateDelay;

    public float AttackCooldown => attackCooldown;

    public float WorkSpeed => workSpeed;
    #endregion

    void Awake()
    {
        myAnim = GetComponent<Animator>();
    }
}
