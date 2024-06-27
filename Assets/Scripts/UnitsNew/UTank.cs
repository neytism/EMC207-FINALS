using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UTank : Units
{
    [SerializeField] private State _currentState = State.Idle;

    private float _attackTime;
    
    private List<UnitType> _priority = new List<UnitType> { UnitType.Clubber , UnitType.Tank};

    private IEnumerator Activate()
    { 
        GetClosestEnemyUnit();
        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);
        ChangeState(State.Chase);

        _attackTime = attackRate;
    }

    private void OnEnable()
    {
        GameManager.OnStartBattleEvent += OnStartBattle;
        receiver.OnAnimationTriggerEvent += Attack;
    }
    
    public override void OnDeath()
    {
        ChangeState(State.Death);
        GameManager.OnStartBattleEvent -= OnStartBattle;
        receiver.OnAnimationTriggerEvent -= Attack;
    }
    
    public override void OnStartBattle()
    {
        StartCoroutine(Activate());
    }

    private void Update()
    {
        if(!isAlive || GameManager.Instance.battleDone) return;
        
        _attackTime += Time.deltaTime;
        
        if (_currentState == State.Chase)
        {
            if (target == null)
            {
                ChangeState(State.Idle);
                return;
            }
            
            var targetPos = target.transform.position;
            float distanceToPlayer = Vector3.Distance( transform.position, targetPos);
            
            agent.SetDestination(targetPos);

            if (distanceToPlayer <= attackRange)
            {
                ChangeState(State.Combat);
            }
            
        } else if (_currentState == State.Combat)
        {
            if (target == null)
            {
                ChangeState(State.Idle);
                return;
            }
            
            var targetPos = target.transform.position;
            float distanceToPlayer = Vector3.Distance( transform.position, targetPos);
            
            transform.forward = Vector3.Lerp(transform.forward, (targetPos - transform.position).normalized, Time.deltaTime * 5f);
            
                
            if (_attackTime >= (attackRate + Random.Range(0f, 2f)))
            {
                animator.SetTrigger("Hit");
                _attackTime = 0;
            }

            if (distanceToPlayer > attackRange)
            {
               ChangeState(State.Chase);
            }
            
        }
        
    }

    public override void Attack()
    {
        if(!isAlive) return;
        if (target == null) return;
    
        foreach (Units unit in GameManager.Instance.AllUnits)
        {
            if (unit.team == targetTeam)
            {
                float dist = Vector3.Distance(unit.transform.position, transform.position);
            
                if (dist <= attackRange)
                {
                    Vector3 direction = (unit.transform.position - transform.position).normalized;
                
                    float dotProduct = Vector3.Dot(transform.forward, direction);
                
                    if (dotProduct > 0)
                    {
                        unit.Hurt(CalculateDamage(), this);
                    }
                }
            }
        }
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Idle:
                agent.speed = 0;
                
                CrossFadeAnimation("Idle", 0.1f);
                break;
            
            case State.Chase:
                agent.speed = speed;
                
                CrossFadeAnimation("Chase", 0.1f);
                break;
            
            case State.Combat:
                agent.speed = 0;
                
                CrossFadeAnimation("Combat", 0.1f);
                break;
            
            case State.Death:
                agent.speed = 0;
                
                animator.SetTrigger("Death");
                break;
            
            default:
                break;
        }
        _currentState = state;
    }
    
    public override void OnHurt()
    {
        
    }
    
    
    
    private enum State
    {
        Idle,
        Chase,
        Combat,
        Death
    }

}
