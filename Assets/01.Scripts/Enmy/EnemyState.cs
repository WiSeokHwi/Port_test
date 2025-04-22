using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController enemy;

    protected EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        
    }
    public virtual void PhysicsUpdate() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    
}
