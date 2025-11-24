using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyMovementController
{
    public void Initialize(EnemyContext context);
    public void LookPlayer();
    public void Stop();
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementController : MonoBehaviour, IEnemyMovementController
{
    [SerializeField] private float attackRange;
    [SerializeField] private float recognitionRange;

    private NavMeshAgent agent;
    private EnemyContext context;

    private Coroutine rotateCoroutine = null;

    public void Initialize(EnemyContext context)
    {
        this.context = context;

        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange;
        agent.autoBraking = true;
        agent.avoidancePriority = (int)attackRange;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    public void LookPlayer()
    {
        if(rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }
        rotateCoroutine = StartCoroutine(RotateToTarget(PlayerController.Instance.transform.position, 0.5f));
    }

    public IEnumerator RotateToTarget(Vector3 targetPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                float step = (180f / duration) * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void Stop()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (context.Controller.State == EnemyState.Move)
        {
            bool inAttackRange = (transform.position - PlayerController.Instance.transform.position).sqrMagnitude <= attackRange * attackRange;
            bool inRecognitionRange = (transform.position - PlayerController.Instance.transform.position).sqrMagnitude <= recognitionRange * recognitionRange;

            if (!inAttackRange && inRecognitionRange) // Move
            {
                agent.SetDestination(PlayerController.Instance.transform.position);
                if (context.AnimationController.CurrentAnimation != EnemyAnimationType.Run) context.AnimationController.Play(EnemyAnimationType.Run);
            }
            else if(!inRecognitionRange) // Out of Recognition
            {
                Stop();
                if (context.AnimationController.CurrentAnimation != EnemyAnimationType.Idle) context.AnimationController.Play(EnemyAnimationType.Idle);
            }
            else if (inAttackRange) // In AttackRange -> Attack
            {
                context.Controller.ChangeState(EnemyState.Attack);
            }
        }
    }
}