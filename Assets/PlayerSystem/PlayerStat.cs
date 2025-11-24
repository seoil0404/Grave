using UnityEngine.Diagnostics;

public static class PlayerStat
{
    public static float Health
    {
        get => PlayerController.PlayerContext.HealthController.Health;
        set => PlayerController.PlayerContext.HealthController.Health = value;
    }

    public static float MaxHealth
    {
        get => PlayerController.PlayerContext.HealthController.MaxHealth;
        set => PlayerController.PlayerContext.HealthController.MaxHealth = value;
    }

    public static float Strength
    {
        get => PlayerController.PlayerContext.CombatController.Strength;
        set => PlayerController.PlayerContext.CombatController.Strength = value;
    }
    public static float Defense
    {
        get => PlayerController.PlayerContext.HealthController.Defense;

        set => PlayerController.PlayerContext.HealthController.Defense = value;
    }
    public static float Speed
    {
        get => PlayerController.PlayerContext.MovementController.Speed;
        set => PlayerController.PlayerContext.MovementController.Speed = value;
    }
    public static float SlideCooldown
    {
        get => PlayerController.PlayerContext.MovementController.SlideCooldown;
        set => PlayerController.PlayerContext.MovementController.SlideCooldown = value;
    }
}



