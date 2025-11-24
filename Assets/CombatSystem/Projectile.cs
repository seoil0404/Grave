using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;

    private SphereCollider projectileCollider;
    private Rigidbody projectileRigidbody;
    private EntityType targetType;
    private float damage;

    public void Initialize(float radius, EntityType targetType, float damage, Vector3 direction, float strength, float destroyDelay)
    {
        projectileCollider = GetComponent<SphereCollider>();
        projectileRigidbody = GetComponent<Rigidbody>();

        projectileCollider.radius = radius;
        
        this.targetType = targetType;
        this.damage = damage;

        direction.Normalize();

        projectileRigidbody.AddForce(direction * strength, ForceMode.Impulse);

        Destroy(gameObject, destroyDelay);
    }

    private void Update()
    {
        Vector3 direction = projectileRigidbody.linearVelocity;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IDamagable>(out var damagableEntity))
        {
            if (damagableEntity.EntityType == targetType)
            {

                GameObject effect = Instantiate(hitEffectPrefab);
                effect.transform.position = transform.position;
                Destroy(effect, 2f);

                damagableEntity.Hit(damage);
                Destroy(gameObject);
            }
        }
    }
}