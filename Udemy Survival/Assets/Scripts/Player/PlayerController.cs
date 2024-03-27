using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float CAMERA_ROTATION_X_LIMIT = 80.0f;      //카메라 X축 회전 상한값



    [Header("컴포넌트")]
    [SerializeField] Camera povCamera;              //플레이어 1인칭 카메라

    CapsuleCollider myCapsuleCollider;
    Rigidbody myRigid;
    CrosshairUI crosshairUI;



    [Header("수치 값")]
    [SerializeField] float walkSpeed;               //걷기 속도

    [SerializeField] float runMultipleRatio;        //뛰기 속도 비율
    float runSpeed;                                 //뛰기 속도 = 걷기 속도 * 뛰기 속도 배율

    [SerializeField] float CrouchMultipleRatio;     //웅크리기 속도 배율
    float crouchSpeed;                              //웅크리기 속도 = 걷기 속도 * 웅크리기 속도 배율

    [SerializeField] float jumpForce;               //점프 힘

    [SerializeField] float crouchPovCameraPosYMinus;//웅크리기 카메라 높이 감소값
    float originPovCameraPosY;                      //1인칭 카메라 기존 Y값
    float crouchPovCameraPosY;                      //웅크린 카메라 Y값

    float currentCameraRotationX;                   //현재 카메라X축 회전값

    [SerializeField] float RotationSensitivity;     //회전 민감도

    /* 플레이어 상태변수 */
    public bool isWalk { get; private set; }
    public bool isRun { get; private set; }
    public bool isCrouch { get; private set; }
    public bool isGround { get; private set; }

    #region 람다식 호출문
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

        //카메라 위아래의 경우 카메라(캐릭터의 머리)만, 좌우의 경우 캐릭터 자체가 움직여야 한다
        CameraRotation();
        CharacterRotation();

        if (isGround)
            SendStateVariable();
    }

    /// <summary>
    /// 바닥에 접촉중인지 체크
    /// </summary>
    void GroundCheck()
    {
        /* myCapsuleCollider.bounds.extents.y = 콜라이더 영역y크기만큼의 절반
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
    /// 이동상태 변환관련
    /// </summary>
    void MovementStateVariable()
    {
        /* 우선순위 : 점프 > 웅크리기 > 달리기
         * 체공 중에는 어떠한 변화도 불가능하다.
         * 체공 중에는 웅크려 있을 수 없다.
         */

        #region 움직임 관련
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

        #region 달리기 관련
        if (Input.GetKey(KeyCode.LeftShift) && (moveDirX != 0 || moveDirZ != 0))
        {
            isRun = true;
            isWalk = false;
        }
        else
            isRun = false;
        #endregion

        #region 웅크리기 관련
        bool _originIsCrouch = isCrouch;//변화되었을 때만 코루틴이 돌 수 있도록
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

        #region 점프 관련
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        #endregion
    }

    /// <summary>
    /// 캐릭터 점프
    /// </summary>
    void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }

    /// <summary>
    /// 캐릭터 이동
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
    /// 카메라 X축 회전값 적용
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
    /// 캐릭터 Y축 회전값 적용
    /// </summary>
    void CharacterRotation()
    {
        /* Quaternion.Euler([Vector3]) : 오일러값(Vector3)을 사원수값으로 치환해준다.
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
    /// 캐릭터 웅크리기 코루틴
    /// </summary>
    IEnumerator IECrouch()
    {
        /* Mathf.Lerp(float a, float b, float t) = a에서 b까지 t비율만큼 움직이게 시킨다.(t가 낮을수록 느리게 적용된다)
         */
        float _crouchTime = 0.0f;

        float _applyPovCameraPosY = isCrouch ? crouchPovCameraPosY : originPovCameraPosY;

        float _posY = povCamera.transform.localPosition.y;

        while (_posY != _applyPovCameraPosY)
        {
            _posY = Mathf.Lerp(_posY, _applyPovCameraPosY, 0.1f);

            povCamera.transform.localPosition = new Vector3(0, _posY, 0);

            _crouchTime += Time.deltaTime;
            if (_crouchTime > 0.1f)//수식 특성 상 _applyPovCameraPosY에 절대 도달할 수 없기에 일정 시간 이후 강제로 도달시킴
                break;

            yield return null;
        }

        povCamera.transform.localPosition = new Vector3(0, _applyPovCameraPosY, 0);
    }
}
