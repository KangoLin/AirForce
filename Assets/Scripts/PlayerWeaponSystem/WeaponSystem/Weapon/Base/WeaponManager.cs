using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase currentWeapon;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentWeapon.TryShoot();
        }
    }
}