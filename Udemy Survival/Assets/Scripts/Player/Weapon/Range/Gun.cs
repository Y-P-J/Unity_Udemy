using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] ParticleSystem muzzleFlash;

    Animator myAnim;



    [Header("수치 값")]
    [SerializeField] string gunName;                //현재 이름

    [SerializeField] int damage;                    //데미지
    [SerializeField] float fireRange;               //사정거리
    [SerializeField] float fireRate;                //연사속도
    [SerializeField] float reloadTime;              //재장전 속도

    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Idle;           //기본 정확도
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Walk;           //걷기 정확도
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_Crouch;         //웅크리기 정확도
    [Range(0.0f, 0.1f)][SerializeField] float accuracy_ADS;            //조준사격 정확도

    [SerializeField] int reloadAmmoCount;           //재장전시 탄알 수
    [SerializeField] int currentAmmoCount;          //현재 탄알 수

    [SerializeField] int maxAmmoCount;              //최대 보유가능한 탄알 수
    [SerializeField] int carryAmmoCount;            //현재 보유중인 탄알 수

    [SerializeField] float recoil_HipFire;          //지향사격 반동 힘
    [SerializeField] float recoil_ADS;              //조준사격 반동 힘

    [SerializeField] Vector3 originForce_ADS;       //조준사격 기존 좌표

    #region 람다식 호출문
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
    /// 총알 사용
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
