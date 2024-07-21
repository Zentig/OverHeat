using System;
using UnityEngine;

public abstract class GunConfig : ScriptableObject
{
    public abstract float CalculateDamage(float baseDamage, int upgradeLevel);
    public abstract float CalculateCooldown(float baseCooldown, int upgradeLevel);
}
