using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] ParticleSystem muzzleFlash;

    Animator myAnim;



    [Header("��ġ ��")]
    [SerializeField] string gunName;                //���� �̸�

    [SerializeField] int damage;                    //������
    [SerializeField] float fireRange;               //�����Ÿ�
    [SerializeField] float fireRate;                //����ӵ�
    [SerializeField] float reloadTime;              //������ �ӵ�

    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Idle;           //�⺻ ��Ȯ��
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Walk;           //�ȱ� ��Ȯ��
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Crouch;         //��ũ���� ��Ȯ��
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_ADS;            //���ػ�� ��Ȯ��

    [SerializeField] int reloadAmmoCount;           //�������� ź�� ��
    [SerializeField] int currentAmmoCount;          //���� ź�� ��

    [SerializeField] int maxAmmoCount;              //�ִ� ���������� ź�� ��
    [SerializeField] int carryAmmoCount;            //���� �������� ź�� ��

    [SerializeField] float recoil_HipFire;          //������ �ݵ� ��
    [SerializeField] float recoil_ADS;              //���ػ�� �ݵ� ��

    [SerializeField] Vector3 originForce_ADS;       //���ػ�� ���� ��ǥ

    #region ���ٽ� ȣ�⹮
    public AudioClip FireSound => fireSound;
    public ParticleSystem MuzzleFlash => muzzleFlash;
    public Animator MyAnim => myAnim;

    public string GunName => gunName;

    public int Damage => damage;
    public float FireRange => fireRange;
    public float FireRate => fireRate;
    public float ReloadTime => reloadTime;

    public float Accuracy_Idle => accuracy_Idle;
    public float Accuracy_Walk => accuracy_Walk;
    public float Accuracy_Crouch => accuracy_Crouch;
    public float Accuracy_ADS => accuracy_ADS;

    public int ReloadAmmoCount => reloadAmmoCount;
    public int CurrentAmmoCount => currentAmmoCount;

    public int MaxAmmoCount => maxAmmoCount;
    public int CarryAmmoCount => carryAmmoCount;

    public float Recoil_HipFire => recoil_HipFire;
    public float Recoil_ADS => recoil_ADS;

    public Vector3 OriginForce_ADS => originForce_ADS;
    #endregion

    void Awake()
    {
        myAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// �Ѿ� ���
    /// </summary>
    public void UseBullet() => currentAmmoCount--;

    public void ReloadBullet()
    {
        carryAmmoCount += currentAmmoCount;
        currentAmmoCount = 0;

        currentAmmoCount = reloadAmmoCount > carryAmmoCount ? carryAmmoCount : reloadAmmoCount;

        carryAmmoCount = Mathf.Clamp(carryAmmoCount - reloadAmmoCount, 0, MaxAmmoCount);
    }
}
