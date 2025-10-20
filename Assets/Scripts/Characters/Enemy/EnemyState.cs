public abstract class EnemyState : CharacterState<EnemyController>
{
    protected EnemyController enemy;

    public EnemyState(EnemyController enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

}
