public abstract class CharacterState<T> where T : CharacterBase
{
    protected T character;

    public CharacterState(T character)
    {
        this.character = character;
    }

    public virtual void Enter()
    {
        if (character.IsDead) return;
    }

    public virtual void Update()
    {
        if (character.IsDead) return;
    }

    public virtual void FixedUpdate()
    {
        if (character.IsDead) return;
    }

    public virtual void Exit()
    {
        if (character.IsDead) return;
    }
}
