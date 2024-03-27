using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] protected Melee currentMelee;



    [Header("수치 값")]
    protected bool isAttack;                      //공격 유무
    protected bool isAttackJudge;                 //공격판정 유무

    protected bool isActivate;



    [Header("그 외")]
    protected RaycastHit hitInfo;

    void Start()
    {
        isAttack = false;
        isAttackJudge = false;
        isActivate = false;
    }

    void Update()
    {
        if (!isActivate)
            return;

        TryAttack();
    }

    /// <summary>
    /// 공격 시도
    /// </summary>
    protected void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (isAttack)
                return;

            StartCoroutine(IEAttack());
        }
    }

    /// <summary>
    /// 오브젝트 충돌 체크
    /// </summary>
    /// <returns></returns>
    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentMelee.AttackRange))
            return true;

        return false;
    }

    public void MeleeChange(Melee _melee)
    {
        currentMelee = _melee;

        currentMelee.transform.localPosition = Vector3.zero;
        currentMelee.gameObject.SetActive(true);

        WeaponManager.ChangeValue(currentMelee.GetComponent<Transform>(), currentMelee.MyAnim);

        Debug.Log("액티브 활성화");
        isActivate = true;
    }

    public void CancelAction()
    {
        Debug.Log("액티브 비활성화");
        isActivate = false;
    }

    /// <summary>
    /// 공격관련 코루틴
    /// </summary>
    /// <returns></returns>
    protected IEnumerator IEAttack()
    {
        isAttack = true;

        currentMelee.MyAnim.SetTrigger("onAttack");

        yield return new WaitForSeconds(currentMelee.AttackActivateDelay);
        isAttackJudge = true;

        StartCoroutine(IEHit());

        yield return new WaitForSeconds(currentMelee.AttackDeactivateDelay);
        isAttackJudge = false;

        yield return new WaitForSeconds(currentMelee.AttackCooldown - currentMelee.AttackActivateDelay - currentMelee.AttackDeactivateDelay);
        isAttack = false;
    }

    /// <summary>
    /// 공격판정관련 코루틴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator IEHit()
    {
        while (isAttackJudge)
        {
            if (CheckObject())
            {
                isAttackJudge = false;
                Debug.Log(hitInfo.transform.name);
            }

            yield return null;
        }
    }
}
