using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    protected EnemyController enemy;
    protected NavMeshAgent agent;
    
    protected EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        agent = enemy.Agent;
    }
    public virtual void PhysicsUpdate() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    
}
