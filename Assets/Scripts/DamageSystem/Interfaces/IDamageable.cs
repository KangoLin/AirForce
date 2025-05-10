using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
}

public struct DamageInfo
{
    public int DamageAmount;
    public DamageType Type;
    public Vector3 HitPoint;
    public GameObject DamageSource;

    public DamageInfo(int amount, DamageType type, Vector3 point, GameObject source)
    {
        DamageAmount = amount;
        Type = type;
        HitPoint = point;
        DamageSource = source;
    }
}