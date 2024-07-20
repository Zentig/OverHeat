using UnityEngine;

[CreateAssetMenu(menuName = "Configs/CannonConfig", fileName = "CannonConfig")]
public class CannonConfig : GunConfig
{
    public override float CalculateCooldown(float baseCooldown, int upgradeLevel) => baseCooldown;
    public override float CalculateDamage(float baseDamage, int upgradeLevel) => baseDamage * upgradeLevel;
}
