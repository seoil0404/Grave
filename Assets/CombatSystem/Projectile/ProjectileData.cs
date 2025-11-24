using UnityEngine;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

[Serializable]
public class ProjectileKeyValuePair
{
    public ProjectileType Key;
    public Projectile Value;
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public List<ProjectileKeyValuePair> Data;
}
