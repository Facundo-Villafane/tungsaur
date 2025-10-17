using UnityEngine;

public abstract class PlayerState : CharacterState<PlayerController>
{
    protected PlayerController player;

    public PlayerState(PlayerController player) : base(player)
    {
        this.player = player;
    }


}
