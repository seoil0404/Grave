using System.Collections;
using UnityEngine;

public class ArcherCombatController : EnemyCombatControllerBase
{
    [SerializeField] private Transform firingTransform;

    protected override void Initialize()
    {
        attackStrategies.Add(Attack1);
    }

    private IEnumerator Attack1()
    {
        context.AnimationController.Play(EnemyAnimationType.Attack1);
        yield return new WaitForSeconds(0.2f);

        Projectile arrow = CombatManager.Instance.GenerateProjectileAttack(ProjectileType.Arrow);
        arrow.transform.position = firingTransform.position;        
        arrow.Initialize(0.05f, EntityType.Player, 1, PlayerController.Instance.gameObject.transform.position - transform.position, 25f, 5f);

        yield return new WaitForSeconds(3.8f);
        EndAttack();
    }
}
