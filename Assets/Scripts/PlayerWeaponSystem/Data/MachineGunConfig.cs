using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Machine Gun Config")]
public class MachineGunConfig : WeaponConfig
{
    [Header("Spread Settings")]
    public int bulletsPerShot = 1;
    public float spreadAngle = 3f;
}