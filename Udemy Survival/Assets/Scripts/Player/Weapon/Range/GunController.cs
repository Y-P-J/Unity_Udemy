using System;
using System.Collections;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] Gun currentGun;            //현재 사용중인 총기
    [SerializeField] Camera povCamera;          //총기의 기준이 될 카메라
    [SerializeField] GameObject hitParticle;    //피격 파티클 프리팹

    PlayerController playerController;
    AudioSource audioSource;
    CrosshairUI crosshairUI;



    [Header("수치 값")]
    Vector3 originPos;                          //기존 위치
    Vector3 applyAim;                           //현재 조준점
    RaycastHit hitInfo;                         //레이 충돌 정보

    float currentFireRate;                      //연발시간 측정
    bool isADS;                                 //조준사격 여부
    bool isReload;                              //리로드 진행중 여부

    bool isActivate;



    #region 람다식 호출문
    public Gun CurrentGun => currentGun;
    public bool IsADS => isADS;
    #endregion



    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        crosshairUI = FindObjectOfType<CrosshairUI>();

        originPos = currentGun.transform.localPosition;
        applyAim = originPos;

        currentFireRate = 0.0f;
        isADS = false;
        isReload = false;

        isActivate = false;
    }

    void Update()
    {
        if (!isActivate)
            return;

        FireRateClac();
        TryFire();
        TryADS();
        TryReload();

        SendStateVariable();
    }

    /// <summary>
    /// 연사속도 계산
    /// </summary>
    void FireRateClac()
    {
        if (currentFireRate <= 0.0f)
            return;

        currentFireRate -= Time.deltaTime;
    }

    /// <summary>
    /// 사격가능 여부 확인
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void TryFire()
    {
        if (currentFireRate > 0.0f || isReload || playerController.isRun == true)
            return;

        if (Input.GetButton("Fire1"))
        {
            if (currentGun.CurrentAmmoCount <= 0)
                TryReload(true);
            else
                Fire();
        }
    }

    Coroutine iERecoil;
    /// <summary>
    /// 사격 관련 함수
    /// </summary>
    void Fire()
    {
        currentGun.UseBullet();
        currentFireRate = currentGun.FireRate;

        if (iERecoil != null)
            StopCoroutine(iERecoil);

        iERecoil = StartCoroutine(IERecoil());
        crosshairUI.SetAnimFireParameters();
        PlaySE(currentGun.FireSound);
        currentGun.MuzzleFlash.Play();
        crosshairUI.SetAnimFireParameters();
        Hit();

        if (currentGun.CurrentAmmoCount % 5 == 0)
            Debug.Log(currentGun.GunName + " 발사됨 / " + currentGun.CurrentAmmoCount + "발 남음");
    }

    void Hit()
    {
        float _accuracy;

        if (isADS) _accuracy = currentGun.Accuracy_ADS;
        else if (playerController.isWalk) _accuracy = currentGun.Accuracy_Walk;
        else if (playerController.isCrouch) _accuracy = currentGun.Accuracy_Crouch;
        else _accuracy = currentGun.Accuracy_Idle;

        Vector3 _rayResult = povCamera.transform.forward + new Vector3(UnityEngine.Random.Range(-_accuracy, _accuracy), UnityEngine.Random.Range(-_accuracy, _accuracy));

        if (Physics.Raycast(povCamera.transform.position, _rayResult, out hitInfo, currentGun.FireRange))
        {
            GameObject _obj = Instantiate(hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(_obj, 1.0f);
        }
    }

    /// <summary>
    /// 조준사격 가능 여부 확인
    /// </summary>
    void TryADS()
    {
        if (isReload || playerController.IsRun)
        {
            isADS = false;
            ADS();
            return;
        }

        if(Input.GetButton("Fire2"))
        {
            isADS = true;
            ADS();
        }
        else if(Input.GetButtonUp("Fire2"))
        {
            isADS = false;
            ADS();
        }
    }

    Coroutine iEADS;
    /// <summary>
    /// 조준사격 관련 함수
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    void ADS()
    {
        currentGun.MyAnim.SetBool("isADS", isADS);

        if (iEADS != null)
            StopCoroutine(iEADS);

        iEADS = StartCoroutine(IEADS());
    }

    /// <summary>
    /// 재장전 관련 함수
    /// </summary>
    /// <param name="_isTry">True일시 추가 입력없이 재장전 됨</param>
    void TryReload(bool _isTry = false)
    {
        if (!isReload && currentGun.CurrentAmmoCount < currentGun.ReloadAmmoCount && currentGun.CarryAmmoCount > 0)
        {
            if (_isTry || Input.GetKeyDown(KeyCode.R))
                Reload();
        }
    }

    Coroutine iEReload;
    void Reload()
    {
        if (iEReload != null)
            StopCoroutine(iEReload);

        iEReload = StartCoroutine(IEReload());
    }

    /// <summary>
    /// SE 사운드 재생
    /// </summary>
    /// <param name="_clip">실행시킬 사운드</param>
    void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    void SendStateVariable()
    {
        crosshairUI.SetAnimGunParameters(isADS);
    }

    public void GunChange(Gun _gun)
    {
        currentGun = _gun;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);

        WeaponManager.ChangeValue(currentGun.GetComponent<Transform>(), currentGun.MyAnim);

        isActivate = true;
    }

    public void CancelAction()
    {
        isActivate = false;

        CancelADS();
        CancelReload();

        void CancelADS()
        {
            isADS = false;

            if (iEADS != null)
                StopCoroutine(iEADS);

            currentGun.MyAnim.SetBool("isADS", isADS);
            crosshairUI.SetAnimGunParameters(isADS);
        }

        void CancelReload()
        {
            isReload = false;

            if (iEReload != null)
                StopCoroutine(iEReload);
        }
    }

    /// <summary>
    /// 총기반동 관련 코루틴
    /// </summary>
    IEnumerator IERecoil()
    {
        Vector3 _targetPos;

        if (isADS)
            _targetPos = new Vector3(currentGun.OriginForce_ADS.x + currentGun.Recoil_ADS, currentGun.OriginForce_ADS.y, currentGun.OriginForce_ADS.z);
        else
            _targetPos = new Vector3(originPos.x + currentGun.Recoil_HipFire, originPos.y, originPos.z);

        int _count = 5;
        while (_count-- > 0)
        {
            if (isADS)
                _targetPos = new Vector3(applyAim.x + currentGun.Recoil_ADS, applyAim.y, applyAim.z);
            else
                _targetPos = new Vector3(applyAim.x + currentGun.Recoil_HipFire, applyAim.y, applyAim.z);

            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, _targetPos, .5f);

            yield return null;
        }

        _count = 5;
        while (_count-- > 0)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, applyAim, .5f);

            yield return null;
        }

        currentGun.transform.localPosition = applyAim;
    }

    /// <summary>
    /// 조준사격 관련 코루틴
    /// </summary>
    IEnumerator IEADS()
    {
        Vector3 _targetPos;
        float _ADSTime = .2f;

        if (isADS)
            _targetPos = currentGun.OriginForce_ADS;
        else
            _targetPos = originPos;


        while (_ADSTime > .0f)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, _targetPos, 0.2f);
            applyAim = currentGun.transform.localPosition;

            _ADSTime -= Time.deltaTime;

            yield return null;
        }

        currentGun.transform.localPosition = _targetPos;
        applyAim = currentGun.transform.localPosition;
    }

    /// <summary>
    /// 재장전 관련 코루틴
    /// </summary>
    IEnumerator IEReload()
    {
        Debug.Log(currentGun.GunName + " 장전중");

        isReload = true;
        currentGun.MyAnim.SetTrigger("onReload");

        yield return new WaitForSeconds(currentGun.ReloadTime);

        currentGun.ReloadBullet();

        isReload = false;

        Debug.Log(currentGun.GunName + " 장전완료");
    }


}
