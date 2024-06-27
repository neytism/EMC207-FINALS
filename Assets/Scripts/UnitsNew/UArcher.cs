using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UArcher : Units
{
    [SerializeField] private State _currentState = State.Idle;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;

    private float _attackTime;

    private List<UnitType> _priority = new List<UnitType> { UnitType.Archer , UnitType.Sorcerer, UnitType.Healer};

    private IEnumerator Activate()
    { 
        GetClosestEnemyUnit();
        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);
        ChangeState(State.Chase);
    }

    private void OnEnable()
    {
        GameManager.OnStartBattleEvent += OnStartBattle;
        receiver.OnAnimationTriggerEvent += Attack;
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
                animator.SetTrigger("Shoot");
                _attackTime = 0;
                GetClosestEnemyUnit();
            }

            if (distanceToPlayer > attackRange)
            {
               ChangeState(State.Chase);
            }
            
        }
        
    }

    public override void Attack()
    {
        GameObject obj = ObjectPool.Instance.PoolObject(_arrowPrefab, _firePoint.position);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.SetValueEffect = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
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
    
    public override void OnDeath()
    {
        ChangeState(State.Death);
        GameManager.OnStartBattleEvent -= OnStartBattle;
        receiver.OnAnimationTriggerEvent -= Attack;
    }
    
    private enum State
    {
        Idle,
        Chase,
        Combat,
        Death
    }

}
