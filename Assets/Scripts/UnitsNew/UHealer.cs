using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Uhealer : Units
{
    [SerializeField] private State _currentState = State.Idle;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;

    private float _attackTime;
    
    [SerializeField] private ParticleSystem _healCircle;
    [SerializeField] private float _healPercentage = 5f;
    
    private float _healTime;
    private int _prevKillCount = 0;

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
        receiver.OnAnimationTriggerEvent += Shoot;
        receiver.OnAnimationStartEvent += PlayHeal;
        receiver.OnAnimationEndEvent += HealNearbyArea;
    }
    
    public override void OnDeath()
    {
        ChangeState(State.Death);
        GameManager.OnStartBattleEvent -= OnStartBattle;
        receiver.OnAnimationTriggerEvent -= Attack;
        receiver.OnAnimationStartEvent -= PlayHeal;
        receiver.OnAnimationEndEvent -= HealNearbyArea;
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
            
            HealChecker();

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
            
            HealChecker();

            if (_attackTime >= (attackRate + Random.Range(0f, 2f)))
            {
                Attack();
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
        animator.SetTrigger("Shoot");
    }

    private void Shoot()
    {
        GameObject obj = ObjectPool.Instance.PoolObject(_arrowPrefab, _firePoint.position);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.SetValueEffect = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
    }

    public void HealChecker()
    {
        _healTime += Time.deltaTime;

        if (_healTime >= healRate)
        {
            if (killCount != _prevKillCount)
            {
                _healTime = 0;
                _prevKillCount = killCount;
                if (!IsTeamMateNear()) return;
                animator.SetTrigger("Heal");
                agent.speed = 0;
            }
        } 
    }
    
    private void PlayHeal()
    {
        if (GameManager.Instance.battleDone) return;
        _healCircle.Play();
    }
    
    private bool IsTeamMateNear()
    {
        Units nearestUnit = null;
        float minDist = visionRange;
        Vector3 currentPos = transform.position;
        foreach (Units u in GameManager.Instance.AllUnits)
        {
            if (!u.isAlive) continue; 
            if (u.team != team) continue;

            float dist = Vector3.Distance(u.transform.position, currentPos);
            
            if (dist < minDist)
            {
                nearestUnit = u;
                minDist = dist;
            }
        }

        return nearestUnit;
    }

    private void HealNearbyArea()
    {
        foreach (Units unit in GameManager.Instance.AllUnits)
        {
            if (unit.team == team)
            {
                float dist = Vector3.Distance(unit.transform.position, transform.position);
                
                if (dist <= visionRange)
                {
                    unit.Heal(_healPercentage);
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
