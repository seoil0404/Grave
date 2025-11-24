using System.Collections;
using UnityEngine;

public interface IEnemyAnimationController
{
    public void Initialize(EnemyContext context);
    public void Play(EnemyAnimationType animationType, float fadeTime = 0.2f);
    public EnemyAnimationType CurrentAnimation { get; }
}

public enum EnemyAnimationType
{
    Idle,
    Run,
    Hit,
    Dead,
    Attack1,
    Attack2
}

public class EnemyAnimationController : MonoBehaviour, IEnemyAnimationController
{
    [SerializeField] private Animator animator;
    
    private EnemyContext context;

    public EnemyAnimationType CurrentAnimation { get; private set; } = EnemyAnimationType.Idle;

    public void Initialize(EnemyContext context)
    {
        this.context = context;
    }

    public void Play(EnemyAnimationType animationType, float fadeTime)
    {
        animator.CrossFadeInFixedTime(animationType.ToString(), fadeTime);
        CurrentAnimation = animationType;
    }
}