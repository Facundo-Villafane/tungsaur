public abstract class EnemyState
{
    protected EnemyController enemy;

    public EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        if (enemy.IsDead) return;
    }

    public virtual void Update()
    {
        if (enemy.IsDead) return;
    }

    public virtual void FixedUpdate()
    {
        if (enemy.IsDead) return;
    }

    public virtual void Exit()
    {
        if (enemy.IsDead) return;
    }
}
