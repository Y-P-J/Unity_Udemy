using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializedDictionary("�̸�", "Gun[]")]
    [SerializeField] SerializedDictionary<string, Gun> guns;
    [SerializedDictionary("�̸�", "Hand[]")]
    [SerializeField] SerializedDictionary<string, Melee> melees;
    [SerializeField] GunController gunController;
    [SerializeField] MeleeController meleeController;

    public static Transform currentWeapon { get; private set; }
    public static Animator currentAnim { get; private set; }



    [Header("��ġ ��")]
    [SerializeField] float changeWeaponDelay;           //��ü���� ������
    [SerializeField] float changeWeaponEndDelay;        //��ü �Ϸ��� ��밡�� ������
    [SerializeField] string currentWeaponType;

    static bool isChangeWeapon;



    #region ���ٽ� ȣ�⹮
    public static bool IsChangeWeapon => isChangeWeapon;
    #endregion



    void Start()
    {
        isChangeWeapon = false;

        StartCoroutine(IEChangeWeapon("MELEE", "Fist"));
    }

    void Update()
    {
        if (isChangeWeapon == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(IEChangeWeapon("MELEE", "Fist"));
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(IEChangeWeapon("MELEE", "Axe"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(IEChangeWeapon("GUN", "SMG"));
            }
        }
    }

    void CancelWeaponAction()
    {
        switch(currentWeaponType)
        {
            case "GUN":
                gunController.CancelAction();
                break;
            case "MELEE":
                meleeController.CancelAction();
                break;
            default:
                Debug.Log($"{gameObject.name}({GetType()}) / �´� currentWeaponType�� �����ϴ�. ������ �߻����� ���ɼ��� �����ϴ�.");
                break;
        }
    }

    void WeaponChange(string _type, string _name)
    {
        WeaponManager.currentWeapon?.gameObject.SetActive(false);

        Debug.Log(_type + "/" + _name);

        if (_type == "GUN")
        {
            gunController.GunChange(guns[_name]);
        }
        else if (_type == "MELEE")
        {
            meleeController.MeleeChange(melees[_name]);
        }
    }

    public static void ChangeValue(Transform _weapon, Animator _anim)
    {
        currentWeapon = _weapon;
        currentAnim = _anim;
    }

    public IEnumerator IEChangeWeapon(string _type, string _name)
    {
        isChangeWeapon = true;

        if (currentWeapon != null)
            currentAnim.SetTrigger("onWeaponOut");

        yield return new WaitForSeconds(changeWeaponDelay);
        CancelWeaponAction();

        yield return new WaitForSeconds(changeWeaponEndDelay);
        WeaponChange(_type, _name);

        currentWeaponType = _type;

        isChangeWeapon = false;
    }
}
