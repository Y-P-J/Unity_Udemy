using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float CAMERA_ROTATION_X_LIMIT = 80.0f;      //ī�޶� X�� ȸ�� ���Ѱ�



    [Header("������Ʈ")]
    [SerializeField] Camera povCamera;              //�÷��̾� 1��Ī ī�޶�

    CapsuleCollider myCapsuleCollider;
    Rigidbody myRigid;
    CrosshairUI crosshairUI;



    [Header("��ġ ��")]
    [SerializeField] float walkSpeed;               //�ȱ� �ӵ�

    [SerializeField] float runMultipleRatio;        //�ٱ� �ӵ� ����
    float runSpeed;                                 //�ٱ� �ӵ� = �ȱ� �ӵ� * �ٱ� �ӵ� ����

    [SerializeField] float CrouchMultipleRatio;     //��ũ���� �ӵ� ����
    float crouchSpeed;                              //��ũ���� �ӵ� = �ȱ� �ӵ� * ��ũ���� �ӵ� ����

    [SerializeField] float jumpForce;               //���� ��

    [SerializeField] float crouchPovCameraPosYMinus;//��ũ���� ī�޶� ���� ���Ұ�
    float originPovCameraPosY;                      //1��Ī ī�޶� ���� Y��
    float crouchPovCameraPosY;                      //��ũ�� ī�޶� Y��

    float currentCameraRotationX;                   //���� ī�޶�X�� ȸ����

    [SerializeField] float RotationSensitivity;     //ȸ�� �ΰ���

    /* �÷��̾� ���º��� */
    public bool isWalk { get; private set; }
    public bool isRun { get; private set; }
    public bool isCrouch { get; private set; }
    public bool isGround { get; private set; }

    #region ���ٽ� ȣ�⹮
    public bool IsRun => isRun;
    #endregion



    void Start()
    {
        myCapsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        crosshairUI = FindObjectOfType<CrosshairUI>();

        runSpeed = walkSpeed * runMultipleRatio;

        crouchSpeed = walkSpeed * CrouchMultipleRatio;

        originPovCameraPosY = povCamera.transform.localPosition.y;
        crouchPovCameraPosY = originPovCameraPosY - crouchPovCameraPosYMinus;

        currentCameraRotationX = 0.0f;

        isWalk = false;
        isRun = false;
        isCrouch = false;
        isGround = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GroundCheck();

        MovementStateVariable();
        Move();

        //ī�޶� ���Ʒ��� ��� ī�޶�(ĳ������ �Ӹ�)��, �¿��� ��� ĳ���� ��ü�� �������� �Ѵ�
        CameraRotation();
        CharacterRotation();

        if (isGround)
            SendStateVariable();
    }

    /// <summary>
    /// �ٴڿ� ���������� üũ
    /// </summary>
    void GroundCheck()
    {
        /* myCapsuleCollider.bounds.extents.y = �ݶ��̴� ����yũ�⸸ŭ�� ����
         */

        isGround = Physics.Raycast(transform.position, Vector3.down, myCapsuleCollider.bounds.extents.y + 0.25f);

        if(!isGround)
        {
            WeaponManager.currentAnim.SetBool("isWalk", false);
            WeaponManager.currentAnim.SetBool("isRun", false);
        }
    }

    float moveDirX;
    float moveDirZ;
    /// <summary>
    /// �̵����� ��ȯ����
    /// </summary>
    void MovementStateVariable()
    {
        /* �켱���� : ���� > ��ũ���� > �޸���
         * ü�� �߿��� ��� ��ȭ�� �Ұ����ϴ�.
         * ü�� �߿��� ��ũ�� ���� �� ����.
         */

        #region ������ ����
        moveDirX = Input.GetAxisRaw("Horizontal");
        moveDirZ = Input.GetAxisRaw("Vertical");

        if (moveDirX == 0 && moveDirZ == 0)
            isWalk = false;
        else
            isWalk = true;
        #endregion

        if (!isGround)
        {
            isCrouch = false;
            StartCoroutine(IECrouch());
            return;
        }

        #region �޸��� ����
        if (Input.GetKey(KeyCode.LeftShift) && (moveDirX != 0 || moveDirZ != 0))
        {
            isRun = true;
            isWalk = false;
        }
        else
            isRun = false;
        #endregion

        #region ��ũ���� ����
        bool _originIsCrouch = isCrouch;//��ȭ�Ǿ��� ���� �ڷ�ƾ�� �� �� �ֵ���
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouch = true;
            isWalk = false;
            isRun = false;
        }
        else
            isCrouch = false;

        if (_originIsCrouch != isCrouch)
            StartCoroutine(IECrouch());
        #endregion

        #region ���� ����
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        #endregion
    }

    /// <summary>
    /// ĳ���� ����
    /// </summary>
    void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }

    /// <summary>
    /// ĳ���� �̵�
    /// </summary>
    void Move()
    {
        float _applySpeed;

        if (isCrouch)
            _applySpeed = crouchSpeed;
        else
            _applySpeed = isRun ? runSpeed : walkSpeed;

        Vector3 _moveHorizontal = transform.right * moveDirX;
        Vector3 _moveVertical = transform.forward * moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * _applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    /// <summary>
    /// ī�޶� X�� ȸ���� ����
    /// </summary>
    void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * RotationSensitivity;

        currentCameraRotationX -= _cameraRotationX;

        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CAMERA_ROTATION_X_LIMIT, CAMERA_ROTATION_X_LIMIT);

        povCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
    }

    /// <summary>
    /// ĳ���� Y�� ȸ���� ����
    /// </summary>
    void CharacterRotation()
    {
        /* Quaternion.Euler([Vector3]) : ���Ϸ���(Vector3)�� ����������� ġȯ���ش�.
         */
        float _yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 _characterRotationY = new Vector3(0.0f, _yRotation, 0.0f) * RotationSensitivity;

        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    void SendStateVariable()
    {
        if (WeaponManager.currentAnim == null)
            return;

        crosshairUI.SetAnimPlayerParameters(isWalk, isRun, isCrouch);

        if (isWalk || isCrouch)
            WeaponManager.currentAnim.SetBool("isWalk", true);
        else
            WeaponManager.currentAnim.SetBool("isWalk", false);

        WeaponManager.currentAnim.SetBool("isRun", isRun);
    }

    /// <summary>
    /// ĳ���� ��ũ���� �ڷ�ƾ
    /// </summary>
    IEnumerator IECrouch()
    {
        /* Mathf.Lerp(float a, float b, float t) = a���� b���� t������ŭ �����̰� ��Ų��.(t�� �������� ������ ����ȴ�)
         */
        float _crouchTime = 0.0f;

        float _applyPovCameraPosY = isCrouch ? crouchPovCameraPosY : originPovCameraPosY;

        float _posY = povCamera.transform.localPosition.y;

        while (_posY != _applyPovCameraPosY)
        {
            _posY = Mathf.Lerp(_posY, _applyPovCameraPosY, 0.1f);

            povCamera.transform.localPosition = new Vector3(0, _posY, 0);

            _crouchTime += Time.deltaTime;
            if (_crouchTime > 0.1f)//���� Ư�� �� _applyPovCameraPosY�� ���� ������ �� ���⿡ ���� �ð� ���� ������ ���޽�Ŵ
                break;

            yield return null;
        }

        povCamera.transform.localPosition = new Vector3(0, _applyPovCameraPosY, 0);
    }
}
