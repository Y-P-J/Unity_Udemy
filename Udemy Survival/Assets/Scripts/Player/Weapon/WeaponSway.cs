using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] GunController gunController;



    [Header("수치 값")]
    [SerializeField] Vector3 limitPos;
    [SerializeField] Vector3 limitPos_ADS;
    [SerializeField] Vector3 smoothSway;
    Vector3 originPos;
    Vector3 currentPos;

    void Start()
    {
        originPos = transform.localPosition;
    }

    void Update()
    {
        TrySway();
    }

    void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
            Swaying();
        else
            BackToOriginPos();
    }

    void Swaying()
    {
        Vector3 _limit;

        if (gunController.IsADS)
            _limit = limitPos_ADS;
        else
            _limit = limitPos;

        currentPos.Set(
            Mathf.Clamp(Mathf.Lerp(currentPos.x, -Input.GetAxisRaw("Mouse X"), smoothSway.x), -_limit.x, _limit.x),
            Mathf.Clamp(Mathf.Lerp(currentPos.y, -Input.GetAxisRaw("Mouse Y"), smoothSway.y), -_limit.y, _limit.y),
            originPos.z);

        transform.localPosition = currentPos;
    }

    void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x * 2);
        transform.localPosition = currentPos;
    }
}
