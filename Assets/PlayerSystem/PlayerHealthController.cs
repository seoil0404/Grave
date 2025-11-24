using System.Collections;
using UnityEngine;

public interface IPlayerHealthController
{
    public void Stop();
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Defense { get; set; }
}

public class PlayerHealthController : MonoBehaviour, IPlayerHealthController, IDamagable
{
    [SerializeField] private float health;
    [SerializeField] private float groggyTime;

    private Coroutine currentGroggyCoroutine = null;

    private float defense = 1f;
    private float maxHealth;

    public EntityType EntityType => EntityType.Player;

    public float Defense
    {
        get => defense;
        set
        {
            defense = value;
            if (value <= 0)
                throw new System.Exception("Defense have to be higher than zero");
        }
    }

    public float Health
    {
        get => health;
        set
        {
            health = value;
            if(health > maxHealth)
                health = maxHealth;

            PlayerController.PlayerContext.AnimationController.SetHealthBar((float)health / (float)maxHealth);
        }
    }

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            float increasedValue = value - maxHealth;
            maxHealth = value;
            Health += increasedValue;

            if (value <= 0)
                throw new System.Exception("MaxHealth need to be higher than zero");

            PlayerController.PlayerContext.AnimationController.SetHealthBar((float)health / (float)maxHealth);
        }
    }

    private void Awake()
    {
        maxHealth = health;
    }

    public void Hit(float damage)
    {
        if (PlayerController.PlayerState == PlayerState.Dead || PlayerController.PlayerContext.MovementController.IsSliding)
            return;

        health -= damage/defense;
        if(health < 0)
            health = 0;

        PlayerController.PlayerContext.AnimationController.SetHealthBar((float)health / (float)maxHealth);

        if (health <= 0)
            PlayerController.ChangeState(PlayerState.Dead);
        else
        {
            if(currentGroggyCoroutine != null)
                StopCoroutine(currentGroggyCoroutine);

            currentGroggyCoroutine = StartCoroutine(GroggyDelay());
        }
    }

    public void Stop()
    {
        if (currentGroggyCoroutine != null)
            StopCoroutine(currentGroggyCoroutine);
    }

    private IEnumerator GroggyDelay()
    {
        PlayerController.ChangeState(PlayerState.Stop);
        PlayerController.PlayerContext.AnimationController.Play(PlayerAnimationType.Hit, 0.1f);

        yield return new WaitForSeconds(groggyTime);
        PlayerController.ChangeState(PlayerState.Normal);
    }
}