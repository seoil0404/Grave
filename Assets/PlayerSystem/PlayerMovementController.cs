using System.Collections;
using UnityEngine;

public interface IPlayerMovementController
{
    Vector2 MoveDirection { get; }
    public void MoveCurve(Vector3 start, Vector3 end, float time = 3f);
    public void SetVelocity(Vector3 velocity);
    public void Stop();
    public bool IsSliding { get; }
    public float Speed { get; set; }
    public float SlideCooldown { get; set; }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour, IPlayerMovementController
{
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float slideCooldown;
    [SerializeField] private float slidingPower;

    [Header("Model")]
    [SerializeField] private Transform modelTransform;

    [Header("Palabolic Curve Setting")]
    [SerializeField] private float height = 3f;

    private Rigidbody playerRigidbody;
    private Vector2 moveDirection;

    private Coroutine currentSlidingCoroutine = null;
    private bool isSlidable = true;
    private bool isSliding = false;

    public Vector2 MoveDirection => moveDirection;

    public bool IsSliding => isSliding;

    public float Speed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float SlideCooldown
    {
        get => slideCooldown;
        set => slideCooldown = value;
    }

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

        HandleSliding();
    }

    private void HandleSliding()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isSlidable)
        {
            if (PlayerController.PlayerState == PlayerState.Normal || PlayerController.PlayerState == PlayerState.Acting)
            {
                PlayerController.PlayerContext.CombatController.Stop();
                PlayerController.PlayerContext.AnimationController.Play(PlayerAnimationType.Sliding, 0.1f);

                Vector2 moveDirection = PlayerController.PlayerContext.MovementController.MoveDirection;
                float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                angle += transform.eulerAngles.y;

                playerRigidbody.AddForce(
                    new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)) * slidingPower, 
                    ForceMode.Impulse
                    );

                currentSlidingCoroutine = StartCoroutine(SlidingCoroutine());
            }
        }
    }

    private IEnumerator SlidingCoroutine()
    {
        PlayerController.ChangeState(PlayerState.Acting);
        isSlidable = false;
        isSliding = true;

        yield return new WaitForSeconds(0.5f);

        isSliding = false;
        PlayerController.ChangeState(PlayerState.Normal);

        yield return new WaitForSeconds(slideCooldown);

        isSlidable = true;
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

    public void Stop()
    {
        SetVelocity(Vector3.zero);
        isSlidable = true;

        if(currentSlidingCoroutine != null )
            StopCoroutine(currentSlidingCoroutine);
    }


    public void SetVelocity(Vector3 velocity)
    {
        playerRigidbody.linearVelocity = velocity;
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

        PlayerController.ChangeState(PlayerState.Stop);

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
