using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float damage;
    private float duration;
    private float timer;
    private PlayerController playerStats;

    public PlayerHitState(PlayerController player, float damage, float duration = 0.5f) : base(player)
    {
        this.damage = damage;
        this.duration = duration;
    }

    public override void Enter()
    {
        timer = 0f;

        if (player.Animator != null)
            player.Animator.SetTrigger("HitTook");

        playerStats = player.GetComponent<PlayerController>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            Debug.Log(playerStats.CurrentHealth);
        }
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            // Volver al estado idle después del golpe
            player.ChangeState(new PlayerIdleState(player));
        }
    }
}
