using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public interface IUpgradeStrategy
{
    public Sprite Sprite { get; }
    public void Apply();
    public string Description { get; }
}

public abstract class UpgradeStrategyBase : IUpgradeStrategy
{
    private static Dictionary<Type, Sprite> itemSprites = new();

    public Sprite Sprite
    {
        get
        {
            if (itemSprites.TryGetValue(GetType(), out var sprite))
                return sprite;

            sprite = Resources.Load<Sprite>($"UpgradeSprite/{GetType().Name}");
            itemSprites.Add(GetType(), sprite);

            return sprite;
        }
    }


    public abstract void Apply();
    public abstract string Description { get; }
}

public class HealthUpgradeStrategy : UpgradeStrategyBase
{
    public override string Description => "heal 30% of max health";

    public override void Apply()
    {
        PlayerStat.Health += PlayerStat.MaxHealth * 0.3f;
    }
}

public class MaxHealthUpgradeStrategy : UpgradeStrategyBase
{
    public override string Description => "increase max health by 10%";

    public override void Apply()
    {
        PlayerStat.MaxHealth *= 1.1f;
    }
}

public class StrengthUpgradeStrategy : UpgradeStrategyBase
{
    public override string Description => "increase strength by 15%";

    public override void Apply()
    {
        PlayerStat.Strength *= 1.15f;
    }
}

public class DefenseUpgradeStrategy : UpgradeStrategyBase
{
    public override string Description => "increase defense by 10%";

    public override void Apply()
    {
        PlayerStat.Defense *= 1.1f;
    }
}

public class SpeedUpgradeStrategy : UpgradeStrategyBase
{
    public override string Description => "increase speed by 10%";

    public override void Apply()
    {
        PlayerStat.Speed *= 1.1f;
    }
}