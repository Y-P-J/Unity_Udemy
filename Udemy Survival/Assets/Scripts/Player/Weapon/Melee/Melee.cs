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

    [Header("������Ʈ")]
    Animator myAnim;

    

    [Header("��ġ ��")]
    [SerializeField] string meleeName;              //�����̸�

    [SerializeField] int attackDamage;              //���ط�
    [SerializeField] float attackRange;             //���� ��Ÿ�
    [SerializeField] float attackCooldown;          //���� ��Ÿ��

    [SerializeField] float workSpeed;               //�۾� �ӵ�

    [SerializeField] float attackActivateDelay;     //���� Ȱ��ȭ ����
    [SerializeField] float attackDeactivateDelay;   //���� ��Ȱ��ȭ ����

    [SerializeField] MeleeType meleeType;

    #region ���ٽ� ȣ�⹮
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
