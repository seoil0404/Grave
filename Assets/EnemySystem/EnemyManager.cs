using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    private static int enemyCount = 0;

    public static int EnemyCount
    {
        get => enemyCount;
        set
        {
            enemyCount = value;
            if(enemyCount <= 0)
            {
                GameManager.Instance.ClearStage();
            }
        }
    }

    [SerializeField] private float spawnCount;
    [SerializeField] private float excludeRadious;
    [SerializeField] private List<EnemyController> enemyPrefabs;

    public float SpawnCount
    {
        get => spawnCount;
        set => spawnCount = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void OnRestart() => enemyCount = 0;

    private void SpawnEnemy(Vector3 position)
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 5f, NavMesh.AllAreas))
        {
            Instantiate(enemyPrefabs[randomIndex]).transform.position = hit.position;
        }
        else throw new System.Exception("NavMesh Does not exist");
        
    }

    public void SpawnEnemys(Square radious)
    {
        var enemySpawnPositions = SpiralPointDistributor.GeneratePoints(radious, count: (int)(spawnCount * radious.Area * 0.001f) + 2, excludeRadius: excludeRadious);
        
        foreach(var point in enemySpawnPositions)
            SpawnEnemy(new Vector3(point.Position.x, radious.Position.y, point.Position.y));
    }
}