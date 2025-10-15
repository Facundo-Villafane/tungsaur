using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;

    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter()
    {
         if (player.IsDead) return;
    }
    public virtual void Update()
    {

        if (player.IsDead) return;
    }
        public virtual void FixedUpdate()
    {

        if (player.IsDead) return;
    }
    
    public virtual void Exit()
    {
        
         if (player.IsDead) return;
    }
}
