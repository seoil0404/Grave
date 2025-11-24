using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCombatController
{
    public float Strength { get; set; }
    public void Stop();
}

public class PlayerCombatController : MonoBehaviour, IPlayerCombatController
{
    [SerializeField] private Transform attackTransform;

    private List<Func<IEnumerator>> normalAttacks = new();
    private int attackCombo = 0;

    private bool allowAdditionalAttack = false;

    private Coroutine currentAttackCoroutine = null;
    private MeleeHitbox currentMeleeHitbox = null;

    private float strength = 1f;

    public float Strength
    {
        get => strength;
        set
        {
            strength = value;
            if (value <= 0f)
                throw new Exception("Strength have to be higher than zero");
        }
    }

    private void Start()
    {
        normalAttacks.Add(NormalAttack1);
        normalAttacks.Add(NormalAttack2);
        normalAttacks.Add(NormalAttack3);
    }

    private void GenerateMeleeHitbox(float scale = 0.5f)
    {
        if(currentMeleeHitbox != null) Destroy(currentMeleeHitbox.gameObject);

        currentMeleeHitbox = CombatManager.Instance.GenerateMeleeAttack();
        currentMeleeHitbox.transform.parent = attackTransform;
        currentMeleeHitbox.transform.localPosition = Vector3.zero;

        currentMeleeHitbox.Initialize(scale, EntityType.Enemy, 1, 0.5f, false);
    }

    private IEnumerator NormalAttack1()
    {
        PlayerController.PlayerContext.AnimationController.Play(PlayerAnimationType.NormalAttack1);

        yield return new WaitForSeconds(0.2f);

        GenerateMeleeHitbox();
        PlayerController.PlayerContext.AnimationController.EnableTrail();

        yield return new WaitForSeconds(0.2f);

        allowAdditionalAttack = true;

        yield return new WaitForSeconds(0.15f);

        PlayerController.ChangeState(PlayerState.Normal);
        PlayerController.PlayerContext.AnimationController.DisableTrail();

        yield return new WaitForSeconds(0.35f);

        

        attackCombo = 0;
    }

    private IEnumerator NormalAttack2()
    {
        PlayerController.PlayerContext.AnimationController.Play(PlayerAnimationType.NormalAttack2);
        GenerateMeleeHitbox();

        PlayerController.PlayerContext.AnimationController.EnableTrail();

        yield return new WaitForSeconds(0.15f);

        
        allowAdditionalAttack = true;

        yield return new WaitForSeconds(0.1f);

        PlayerController.ChangeState(PlayerState.Normal);
        PlayerController.PlayerContext.AnimationController.DisableTrail();

        yield return new WaitForSeconds(0.35f);

        

        attackCombo = 0;
    }

    private IEnumerator NormalAttack3()
    {
        PlayerController.PlayerContext.AnimationController.Play(PlayerAnimationType.NormalAttack3);
        PlayerController.PlayerContext.AnimationController.EnableTrail();

        yield return new WaitForSeconds(0.2f);

        GenerateMeleeHitbox(0.75f);

        yield return new WaitForSeconds(0.4f);

        PlayerController.PlayerContext.AnimationController.DisableTrail();

        yield return new WaitForSeconds(0.1f);

        PlayerController.ChangeState(PlayerState.Normal);
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(PlayerController.PlayerState == PlayerState.Normal || allowAdditionalAttack)
            {
                allowAdditionalAttack = false;

                PlayerController.ChangeState(PlayerState.Acting);

                if(currentAttackCoroutine != null)
                    StopCoroutine(currentAttackCoroutine);

                currentAttackCoroutine = StartCoroutine(normalAttacks[attackCombo].Invoke());
                
                attackCombo = (attackCombo + 1) % normalAttacks.Count;
            }
        }
    }

    public void Stop()
    {
        if(currentAttackCoroutine != null)
            StopCoroutine(currentAttackCoroutine);

        PlayerController.PlayerContext.AnimationController.DisableTrail();
        allowAdditionalAttack = false;
        attackCombo = 0;
    }
}
