using System.Collections;
using UnityEngine;

public interface IPlayerMovementController
{
    Vector2 MoveDirection { get; }
    void MoveCurve(Vector3 start, Vector3 end, float time = 3f);
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour, IPlayerMovementController
{
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed;

    [Header("Palabolic Curve Setting")]
    [SerializeField] private float height = 3f;

    private Rigidbody playerRigidbody;

    private Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;

    public void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(PlayerController.PlayerState == PlayerState.Normal)
            HandleInput();
        if (PlayerController.PlayerState == PlayerState.Cinematic)
            HandleCurveMove();
    }

    private void HandleInput()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        moveDirection.x = xAxis;
        moveDirection.y = yAxis;

        Vector3 moveDir = new Vector3(moveDirection.x, 0f, moveDirection.y);
        moveDir = transform.TransformDirection(moveDir);

        playerRigidbody.linearVelocity = new Vector3(
            moveDir.x * moveSpeed,
            playerRigidbody.linearVelocity.y,
            moveDir.z * moveSpeed
        );

    }

    #region Curve

    private Vector3 start, end = Vector3.zero;

    private float curveTime = 0f;
    private float leftCurveTime = 0f;

    private Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float rate)
    {
        Vector3 mid = Vector3.Lerp(start, end, rate);
        float parabola = 4 * height * rate * (1 - rate);
        mid.y += parabola;
        return mid;
    }

    public void MoveCurve(Vector3 start, Vector3 end, float time = 2)
    {
        this.start = start;
        this.end = end;
        leftCurveTime = time;
        curveTime = time;

        StartCoroutine(MoveCurveDelay());
        PlayerController.PlayerContext.AnimationController.OnMoveCurve(time);
    }

    private IEnumerator MoveCurveDelay()
    {
        yield return new WaitForSeconds(0.5f);

        PlayerController.ChangeState(PlayerState.Cinematic);
    }

    private void HandleCurveMove()
    {
        leftCurveTime -= Time.deltaTime;
        if (leftCurveTime < 0f)
        {
            PlayerController.ChangeState(PlayerState.Normal);
            return;
        }

        transform.position = GetParabolaPoint(start, end, 1 - (leftCurveTime / curveTime));
    }

    #endregion
}
