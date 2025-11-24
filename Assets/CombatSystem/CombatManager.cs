using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ProjectileType
{
    Arrow
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {  get; private set; }

    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private MeleeHitbox meleeHitboxPrefab;

    private Dictionary<ProjectileType, Projectile> projectilePrefabs = new();

    private void Awake()
    {
        Instance = this;

        foreach(var item in projectileData.Data)
        {
            projectilePrefabs.Add(item.Key, item.Value);
        }
    }

    public MeleeHitbox GenerateMeleeAttack()
    {
        return Instantiate(meleeHitboxPrefab);
    }

    public Projectile GenerateProjectileAttack(ProjectileType projectileType)
    {
        return Instantiate(projectilePrefabs[projectileType]);
    }    
}
