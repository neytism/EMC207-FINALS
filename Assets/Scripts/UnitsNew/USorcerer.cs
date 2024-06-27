using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class USorcerer : Units
{
    [SerializeField] private State _currentState = State.Idle;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePointRight;
    [SerializeField] private Transform _firePointLeft;
    [SerializeField] private float _arrowSpeed;

    private float _attackTime;
    
    [SerializeField] private float _skillRate = 10f;
    private float _skillTime;
    
    //[SerializeField] private ParticleSystem _healCircle;
    
    private float _healTime;
    
    //private int _prevKillCount = 0;
    private bool _isRightHandUsed = false;
    
    private List<UnitType> _priority = new List<UnitType> { UnitType.Tank , UnitType.Sorcerer, UnitType.Healer};
    

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
        receiver.OnAnimationStartEvent += TurnOffLayerWeight;
        receiver.OnAnimationEndEvent += TurnOnLayerWeight;
    }
    
    public override void OnDeath()
    {
        ChangeState(State.Death);
        GameManager.OnStartBattleEvent -= OnStartBattle;
        receiver.OnAnimationTriggerEvent -= Shoot;
        receiver.OnAnimationStartEvent -= TurnOffLayerWeight;
        receiver.OnAnimationEndEvent -= TurnOnLayerWeight;
    }
    
    public override void OnStartBattle()
    {
        StartCoroutine(Activate());
    }

    private void Update()
    {
        if(!isAlive || GameManager.Instance.battleDone) return;
        
        _attackTime += Time.deltaTime;
        _skillTime += Time.deltaTime;
        
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
                //animator.SetTrigger("Shoot");
                
                //make random shits here
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
        if (_skillTime >= (_skillRate + Random.Range(0f, 3f)))
        {
            animator.SetTrigger("Burst");
            _skillTime = -5f;
            _attackTime = -5f;
            return;
        }
        
        if (_isRightHandUsed)
        {
            animator.SetTrigger("ShootLeft");
        }else
        {
            animator.SetTrigger("ShootRight");
        }
    }

    private void TurnOffLayerWeight()
    {
        animator.SetLayerWeight(1, 0);
    }
    
    private void TurnOnLayerWeight()
    {
        animator.SetLayerWeight(1, 1);
        animator.ResetTrigger("ShootLeft");
        animator.ResetTrigger("ShootRight");
    }

    public void Shoot()
    {
        Transform point;
        if (_isRightHandUsed)
        {
            point = _firePointLeft;
            _isRightHandUsed = false;
        }else
        {
            point = _firePointRight;
            _isRightHandUsed = true;
        }
        
        GameObject obj = ObjectPool.Instance.PoolObject(_arrowPrefab, point.position);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.SetValueEffect = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
    }

    public bool IsTeamMateNear()
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
                
                CrossFadeAnimation("Chase", 0.01f);
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
