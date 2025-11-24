using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyCombatController
{
    public void Initialize(EnemyContext context);
    public void Attack();
    public void Stop();
}

public abstract class EnemyCombatControllerBase : MonoBehaviour, IEnemyCombatController
{
    protected List<Func<IEnumerator>> attackStrategies = new();
    protected EnemyContext context;

    private Coroutine currentAttackCoroutine = null;

    public void Attack()
    {
        int randomIndex = UnityEngine.Random.Range(0, attackStrategies.Count);
        currentAttackCoroutine = StartCoroutine(attackStrategies[randomIndex].Invoke());
        context.MovementController.LookPlayer();
    }

    protected void EndAttack()
    {
        context.Controller.ChangeState(EnemyState.Move);
    }

    public void Initialize(EnemyContext context)
    {
        this.context = context;
        
        Initialize();
    }

    public void Stop()
    {
        if(currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
        }
    }

    /// <summary>
    /// Initialize 'attackStrategies' List
    /// </summary>
    protected abstract void Initialize();
}
