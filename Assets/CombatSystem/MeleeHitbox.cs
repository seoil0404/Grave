using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;

    private SphereCollider meleeCollider;
    private EntityType targetType;
    private float damage;
    private bool isDisposable;

    private HashSet<IDamagable> damagables = new();

    public void Initialize(float radius, EntityType targetType, float damage, float destroyDelay, bool isDisposable = true)
    {
        meleeCollider = GetComponent<SphereCollider>();
        meleeCollider.radius = radius;
        this.targetType = targetType;
        this.damage = damage;
        this.isDisposable = isDisposable;

        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IDamagable>(out var damagableEntity))
        {
            if(damagableEntity.EntityType == targetType && !damagables.TryGetValue(damagableEntity, out var _))
            {
                damagableEntity.Hit(damage);

                GameObject effect = Instantiate(hitEffectPrefab);
                effect.transform.position = transform.position;
                Destroy(effect, 2f);
                
                if(isDisposable) Destroy(gameObject);
                else
                {
                    damagables.Add(damagableEntity);
                }
            }
        }
    }
}