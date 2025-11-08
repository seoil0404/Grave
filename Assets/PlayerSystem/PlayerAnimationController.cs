
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public interface IPlayerAnimationController
{
    void OnMoveCurve(float time);
}

public class PlayerAnimationController : MonoBehaviour, IPlayerAnimationController
{
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float rotateSpeed = 720f;

    private float currentAngle = 0;

    private void Update()
    {
        if (PlayerController.PlayerState == PlayerState.Normal)
        {
            HandleRotation();
            HandleAnimation();
        }
    }

    private void HandleAnimation()
    {
        if (PlayerController.PlayerContext.MovementController.MoveDirection.sqrMagnitude < 0.001f)
            modelAnimator.SetBool("IsRunning", false);
        else
            modelAnimator.SetBool("IsRunning", true);
    }

    private void HandleRotation()
    {
        Vector2 moveDirection = PlayerController.PlayerContext.MovementController.MoveDirection;
        currentAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;

        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        float targetAngle = Mathf.LerpAngle(modelTransform.localEulerAngles.y, currentAngle, rotateSpeed * Time.deltaTime);

        modelTransform.localRotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    public void OnMoveCurve(float landingDelay)
    {
        StartCoroutine(OnMoveCurveCoroutine(landingDelay));
    }

    private IEnumerator OnMoveCurveCoroutine(float landingDelay)
    {
        modelAnimator.Play("Jump");
        yield return new WaitForSeconds(landingDelay + 0.3f);
        modelAnimator.Play("Land");
    }
}