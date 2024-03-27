using System;
using System.Collections;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField] Gun currentGun;            //���� ������� �ѱ�
    [SerializeField] Camera povCamera;          //�ѱ��� ������ �� ī�޶�
    [SerializeField] GameObject hitParticle;    //�ǰ� ��ƼŬ ������

    PlayerController playerController;
    AudioSource audioSource;
    CrosshairUI crosshairUI;



    [Header("��ġ ��")]
    Vector3 originPos;                          //���� ��ġ
    Vector3 applyAim;                           //���� ������
    RaycastHit hitInfo;                         //���� �浹 ����

    float currentFireRate;                      //���߽ð� ����
    bool isADS;                                 //���ػ�� ����
    bool isReload;                              //���ε� ������ ����

    bool isActivate;



    #region ���ٽ� ȣ�⹮
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
    /// ����ӵ� ���
    /// </summary>
    void FireRateClac()
    {
        if (currentFireRate <= 0.0f)
            return;

        currentFireRate -= Time.deltaTime;
    }

    /// <summary>
    /// ��ݰ��� ���� Ȯ��
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
    /// ��� ���� �Լ�
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
            Debug.Log(currentGun.GunName + " �߻�� / " + currentGun.CurrentAmmoCount + "�� ����");
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
    /// ���ػ�� ���� ���� Ȯ��
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
    /// ���ػ�� ���� �Լ�
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
    /// ������ ���� �Լ�
    /// </summary>
    /// <param name="_isTry">True�Ͻ� �߰� �Է¾��� ������ ��</param>
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
    /// SE ���� ���
    /// </summary>
    /// <param name="_clip">�����ų ����</param>
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
    /// �ѱ�ݵ� ���� �ڷ�ƾ
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
    /// ���ػ�� ���� �ڷ�ƾ
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
    /// ������ ���� �ڷ�ƾ
    /// </summary>
    IEnumerator IEReload()
    {
        Debug.Log(currentGun.GunName + " ������");

        isReload = true;
        currentGun.MyAnim.SetTrigger("onReload");

        yield return new WaitForSeconds(currentGun.ReloadTime);

        currentGun.ReloadBullet();

        isReload = false;

        Debug.Log(currentGun.GunName + " �����Ϸ�");
    }


}
