using UnityEngine;

public class PlayerContext
{
    public IPlayerCombatController CombatController { get; set; }
    public IPlayerMovementController MovementController { get; set; }
    public IPlayerAnimationController AnimationController { get; set; }
}

public enum PlayerState
{
    Normal,
    Cinematic,
    Dead,
    UIOnly,
    Acting // Normal Attack, Skills
}

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public static PlayerContext PlayerContext { get; private set; }
    public static PlayerState PlayerState { get; private set; } = PlayerState.Normal;
    
    public static void ChangeState(PlayerState state) => PlayerState = state;


    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

        PlayerContext = new();

        var combatController = GetComponent<PlayerCombatController>();
        PlayerContext.CombatController = combatController;

        var movementController = GetComponent<PlayerMovementController>();
        PlayerContext.MovementController = movementController;

        var animationController = GetComponent<PlayerAnimationController>();
        PlayerContext.AnimationController = animationController;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
