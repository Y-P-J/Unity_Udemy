using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField] protected Melee currentMelee;



    [Header("��ġ ��")]
    protected bool isAttack;                      //���� ����
    protected bool isAttackJudge;                 //�������� ����

    protected bool isActivate;



    [Header("�� ��")]
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
    /// ���� �õ�
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
    /// ������Ʈ �浹 üũ
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

        Debug.Log("��Ƽ�� Ȱ��ȭ");
        isActivate = true;
    }

    public void CancelAction()
    {
        Debug.Log("��Ƽ�� ��Ȱ��ȭ");
        isActivate = false;
    }

    /// <summary>
    /// ���ݰ��� �ڷ�ƾ
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
    /// ������������ �ڷ�ƾ
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
