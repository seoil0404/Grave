using System;
using UnityEngine;

public class PlayerContext
{
    public IPlayerCombatController CombatController { get; set; }
    public IPlayerMovementController MovementController { get; set; }
    public IPlayerAnimationController AnimationController { get; set; }
    public IPlayerHealthController HealthController { get; set; }
}

public enum PlayerState
{
    Normal,
    Cinematic,
    Dead,
    UIOnly,
    Stop,
    Acting, // Normal Attack, Skills
    Start
}

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerHealthController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public static PlayerContext PlayerContext { get; private set; }
    public static PlayerState PlayerState { get; private set; } = PlayerState.Start;

    public static void ChangeState(PlayerState state)
    {
        if (PlayerState == PlayerState.Dead)
            return;

        PlayerState = state;

        switch(state)
        {
            case PlayerState.Normal:
                PlayerContext.AnimationController.Play(PlayerAnimationType.Idle);
                break;
            case PlayerState.Acting:
                PlayerContext.MovementController.Stop();
                break;
            case PlayerState.Cinematic:
                PlayerContext.CombatController.Stop();
                break;
            case PlayerState.Stop:
                PlayerContext.CombatController.Stop();
                PlayerContext.MovementController.Stop();
                break;
            case PlayerState.Dead:
                PlayerContext.CombatController.Stop();
                PlayerContext.AnimationController.Play(PlayerAnimationType.Dead);
                PlayerContext.MovementController.Stop();
                GameManager.Instance.PlayerDead();
                break;
        }
    }

    public static void OnReStart() => PlayerState = PlayerState.Start;


    private void Awake()
    {
        Instance = this;

        PlayerContext = new();

        var combatController = GetComponent<IPlayerCombatController>();
        PlayerContext.CombatController = combatController;

        var movementController = GetComponent<IPlayerMovementController>();
        PlayerContext.MovementController = movementController;

        var animationController = GetComponent<IPlayerAnimationController>();
        PlayerContext.AnimationController = animationController;

        var healthController = GetComponent<IPlayerHealthController>();
        PlayerContext.HealthController = healthController;
    }

    private void Update()
    {

    }
}
