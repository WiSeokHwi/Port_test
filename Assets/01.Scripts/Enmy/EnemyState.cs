using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    protected EnemyController enemy;
    protected NavMeshAgent agent;
    protected GameObject[] targets;
    
    protected EnemyState(EnemyController enemy)
    {
        this.enemy = enemy;
        this.agent = enemy.Agent;
        targets = enemy.targets;
    }

    public virtual void Enter() {}
    public virtual void PhysicsUpdate() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    
}
