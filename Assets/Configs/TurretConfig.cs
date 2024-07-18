using UnityEngine;

[CreateAssetMenu(menuName = "Configs/TurretConfig", fileName = "TurretConfig")]
public class TurretConfig : GunConfig
{
    public override float CalculateCooldown(float baseCooldown, int upgradeLevel) => baseCooldown - 0.05f * upgradeLevel;
    public override float CalculateDamage(float baseDamage, int upgradeLevel) => baseDamage;
}
