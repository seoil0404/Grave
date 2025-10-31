using UnityEngine;

public class PlayerContext
{
    public IPlayerCombatController CombatController { get; set; }
    public IPlayerMovementController MovementController { get; set; }
}

public class PlayerState
{

}

public interface IManagedBehaviour
{
    void OnStart();
    void OnUpdate();
}

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public static PlayerContext PlayerContext { get; private set; }
    public static PlayerState PlayerState { get; private set; }

    private IManagedBehaviour combatController;
    private IManagedBehaviour movementController;


    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

        PlayerContext = new();
        PlayerState = new();

        var _combatController = GetComponent<PlayerCombatController>();
        PlayerContext.CombatController = _combatController;
        combatController = _combatController;

        var _movementController = GetComponent<PlayerMovementController>();
        PlayerContext.MovementController = _movementController;
        movementController = _movementController;
    }

    private void Start()
    {
        combatController.OnStart();
        movementController.OnStart();
    }

    private void Update()
    {
        combatController.OnUpdate();
        movementController.OnUpdate();
    }
}
