using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("ÄÄÆ÷³ÍÆ®")]
    [SerializeField] GunController gunController;
    [SerializeField] TMP_Text currentAmmoText;
    [SerializeField] TMP_Text carryAmmoText;

    GameObject ammoUI;



    void Start()
    {
        ammoUI = GetComponent<GameObject>();
    }

    void Update()
    {
        UpdateAmmoText();
    }

    void UpdateAmmoText()
    {
        currentAmmoText.text = gunController.CurrentGun.CurrentAmmoCount.ToString();
        carryAmmoText.text = gunController.CurrentGun.CarryAmmoCount.ToString();
    }
}
