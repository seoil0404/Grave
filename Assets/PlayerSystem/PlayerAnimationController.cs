
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPlayerAnimationController
{
    public void Play(PlayerAnimationType animationType, float fadeTime = 0.2f);
    public void OnMoveCurve(float time);
    public void EnableTrail();
    public void DisableTrail();
    public void SetHealthBar(float rate);
    public PlayerAnimationType CurrentAnimation { get; }
}

public enum PlayerAnimationType
{
    Idle,
    Run,
    Jump,
    Land,
    QSkill,
    Sliding,
    Hit,
    Dead,
    NormalAttack1,
    NormalAttack2,
    NormalAttack3
}

public class PlayerAnimationController : MonoBehaviour, IPlayerAnimationController
{
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private float rotateSpeed = 720f;

    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject jumpChargingEffect;
    [SerializeField] private GameObject jumpEffect;

    [SerializeField] private List<TrailRenderer> trailRenderers;

    private float currentAngle = 0;

    public PlayerAnimationType CurrentAnimation { get; private set; } = PlayerAnimationType.Idle;

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

        Vector3 effectOffset = Vector3.down * 1f;

        GameObject chargingEffect = Instantiate(jumpChargingEffect);
        chargingEffect.transform.position = transform.position + effectOffset;
        yield return new WaitForSeconds(0.1f);
        
        Destroy(chargingEffect.gameObject);
        GameObject jumpEffect = Instantiate(jumpChargingEffect);
        jumpEffect.transform.position = transform.position + effectOffset;

        yield return new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(landingDelay);

        Destroy(jumpEffect);
        modelAnimator.Play("Land");
    }

    public void Play(PlayerAnimationType animationType, float fadeTime = 0.2F)
    {
        modelAnimator.CrossFadeInFixedTime(animationType.ToString(), fadeTime);
        CurrentAnimation = animationType;
    }

    public void EnableTrail()
    {
        trailRenderers.ForEach(trail => trail.emitting = true);
    }

    public void DisableTrail()
    {
        trailRenderers.ForEach(trail => trail.emitting = false);
    }

    public void SetHealthBar(float rate)
    {
        healthBar.fillAmount = rate;
    }
}