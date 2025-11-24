using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Move,
    Die,
    Rest,
    Attack,
    Hit
}

public class EnemyContext
{
    public IEnemyController Controller { get; set; }
    public IEnemyMovementController MovementController { get; set; }
    public IEnemyCombatController CombatController { get; set; }
    public IEnemyAnimationController AnimationController { get; set; }
    public IEnemyHealthController HealthController { get; set; }
}

public interface IEnemyController
{
    public EnemyState State { get; }
    public void ChangeState(EnemyState state);
}

[RequireComponent(typeof(EnemyMovementController))]
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(EnemyHealthController))]
public class EnemyController : MonoBehaviour, IEnemyController
{
    protected NavMeshAgent agent;
    private EnemyContext context = new();

    public EnemyState State { get; private set; }

    private void Start()
    {
        EnemyManager.EnemyCount++;

        context.Controller = this;

        context.MovementController = GetComponent<IEnemyMovementController>();
        context.MovementController.Initialize(context);

        if (TryGetComponent(out IEnemyCombatController combatController))
        {
            context.CombatController = combatController;
            context.CombatController.Initialize(context);
        }
        else throw new Exception("MissingCombatControllerException : The Enemy object does not contain a CombatController component, which is required for combat behavior.");

        context.AnimationController = GetComponent<IEnemyAnimationController>();
        context.AnimationController.Initialize(context);

        context.HealthController = GetComponent<IEnemyHealthController>();
        context.HealthController.Initialize(context);

        ChangeState(EnemyState.Rest);
    }

    public void ChangeState(EnemyState state)
    {
        State = state;

        switch (state)
        {
            case EnemyState.Attack:
                context.MovementController.Stop();
                context.CombatController.Attack();
                break;
            case EnemyState.Move:
                break;
            case EnemyState.Hit:
                context.MovementController.Stop();
                context.CombatController.Stop();
                context.AnimationController.Play(EnemyAnimationType.Hit);
                break;
            case EnemyState.Die:
                context.MovementController.Stop();
                context.CombatController.Stop();
                Destroy(GetComponent<Collider>());
                EnemyManager.EnemyCount--;
                ExperienceManager.Instance.AddExperience();
                context.AnimationController.Play(EnemyAnimationType.Dead);
                Destroy(gameObject, 4f);
                break;
            case EnemyState.Rest:
                context.MovementController.Stop();
                context.CombatController.Stop();
                context.HealthController.Stop();
                context.AnimationController.Play(EnemyAnimationType.Idle);
                break;
        }
    }

    public void StartEnemy()
    {
        ChangeState(EnemyState.Move);
    }
}
