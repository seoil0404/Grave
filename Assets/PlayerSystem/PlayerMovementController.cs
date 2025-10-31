using UnityEngine;

public interface IPlayerMovementController
{
    Vector2 MoveDirection { get; }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour, IPlayerMovementController, IManagedBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody playerRigidbody;

    public Vector2 MoveDirection => moveDirection;
    private Vector2 moveDirection;

    public void OnStart()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void OnUpdate()
    {
        HandleInput();
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
}
