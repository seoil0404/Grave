public interface IDamagable
{
    public EntityType EntityType { get; }
    public void Hit(float damage);
}

public enum EntityType
{
    Player,
    Enemy
}