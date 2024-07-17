public interface IUpgradable
{
    public int UpgradeLevel { get; set; }
    public int MaxUpgradeLevel { get; set; }
    public UpgradeTypes UpgradeType { get; set; }
}
