using System.Collections;
using UnityEngine;

public class ZombieCombatController : EnemyCombatControllerBase
{
    [SerializeField] private Transform attack1HitboxParent;
    [SerializeField] private Transform attack2HitboxParent;

    [SerializeField] private int damage;

    protected override void Initialize()
    {
        attackStrategies.Add(Attack1);
        attackStrategies.Add(Attack2);
    }

    private IEnumerator Attack1()
    {
        context.AnimationController.Play(EnemyAnimationType.Attack1);
        yield return new WaitForSeconds(1f);

        MeleeHitbox meleeHitbox = CombatManager.Instance.GenerateMeleeAttack();
        meleeHitbox.transform.parent = attack1HitboxParent;
        meleeHitbox.transform.localPosition = Vector3.zero;

        meleeHitbox.Initialize(1f, EntityType.Player, damage, 0.3f);

        yield return new WaitForSeconds(1f);
        EndAttack();
    }

    private IEnumerator Attack2()
    {
        context.AnimationController.Play(EnemyAnimationType.Attack2);
        yield return new WaitForSeconds(1f);

        MeleeHitbox meleeHitbox = CombatManager.Instance.GenerateMeleeAttack();
        meleeHitbox.transform.parent = attack2HitboxParent;
        meleeHitbox.transform.localPosition = Vector3.zero;

        meleeHitbox.Initialize(.3f, EntityType.Player, damage, 0.3f);

        yield return new WaitForSeconds(1f);
        EndAttack();
    }
}
