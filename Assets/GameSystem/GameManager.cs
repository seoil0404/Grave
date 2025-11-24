using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsStarted { get; private set; } = false;
    public static int StageCount { get; private set; } = 1;

    [SerializeField] private CameraController cameraController;

    [Header("Prefabs")]
    [SerializeField] private EndView endViewPrefab;
    [SerializeField] private StageView stageViewPrefab;

    private void Awake()
    {
        if(Instance != null)
            return;

        Instance = this;
    }

    public void ClearStage()
    {
        StageCount++;
        EnemyManager.Instance.SpawnCount *= 1.2f;
        StartCoroutine(ClearStageDelay());
    }

    private IEnumerator ClearStageDelay()
    {
        yield return new WaitForSeconds(2f);


        MoveStage();
    }

    private void MoveStage()
    {
        if (MapManager.CurrentSquare.TargetNode == null)
            MapManager.Instance.AddMap();
        MapManager.CurrentSquare = MapManager.CurrentSquare.TargetNode.Square;

        PlayerController.PlayerContext.MovementController.MoveCurve(
            PlayerController.Instance.transform.position,
            MapManager.CurrentSquare.Position + Vector3.up * 2,
            1f
            );

        GrassManager.Instance.DrawGrass(MapManager.CurrentSquare);
        FenceManager.Instance.DrawFence(MapManager.CurrentSquare);
        EnemyManager.Instance.SpawnEnemys(MapManager.CurrentSquare);

        StartCoroutine(InstanceStageViewDelay());
        StartCoroutine(EnemyStartDelay());
    }

    private IEnumerator InstanceStageViewDelay()
    {
        yield return new WaitForSeconds(1.75f);
        Instantiate(stageViewPrefab);
    }

    private IEnumerator EnemyStartDelay()
    {
        yield return new WaitForSeconds(3f);

        FindObjectsByType<EnemyController>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(t => { if (t.State != EnemyState.Die) t.ChangeState(EnemyState.Move); });
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {

    }

    public void StartGame()
    {
        IsStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
        HealthBarCanvasController.Instance.EnableHealthBar();
        CanvasController.Instance.EnableCanvas();
        cameraController.OnStartGame();
        MoveStage();
    }

    public void PlayerDead()
    {
        IsStarted = false;
        FindObjectsByType<EnemyController>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(t => { if (t.State != EnemyState.Die) t.ChangeState(EnemyState.Rest); });

        Cursor.lockState = CursorLockMode.None;

        Instantiate(endViewPrefab);
    }

    public void ReStart()
    {
        StageCount = 1;
        EnemyManager.Instance.OnRestart();
        PlayerController.OnReStart();
    }
}
