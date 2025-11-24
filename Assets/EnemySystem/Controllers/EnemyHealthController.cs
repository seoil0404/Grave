using System.Collections;
using UnityEngine;

public interface IEnemyHealthController
{
    public void Initialize(EnemyContext context);
    public void Stop();
}

public class EnemyHealthController : MonoBehaviour, IEnemyHealthController, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private float groggyTime;

    private EnemyContext context;
    
    private Coroutine currentGroggyCoroutine = null;

    public EntityType EntityType => EntityType.Enemy;

    public void Hit(float damage)
    {
        if (context.Controller.State == EnemyState.Die)
            return;

        health -= damage;

        if (health <= 0)
        {
            if(currentGroggyCoroutine != null)
                StopCoroutine(currentGroggyCoroutine);

            context.Controller.ChangeState(EnemyState.Die);
        }
        else
        {
            if(currentGroggyCoroutine != null)
                StopCoroutine(currentGroggyCoroutine);

            currentGroggyCoroutine = StartCoroutine(Groggy());
        }
    }

    private IEnumerator Groggy()
    {
        context.Controller.ChangeState(EnemyState.Hit);
        
        yield return new WaitForSeconds(groggyTime);

        context.Controller.ChangeState(EnemyState.Move);
    }

    public void Initialize(EnemyContext context)
    {
        this.context = context;
    }

    public void Stop()
    {
        if (currentGroggyCoroutine != null)
            StopCoroutine(currentGroggyCoroutine);
    }
}
