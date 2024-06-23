using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Units : MonoBehaviour
{
    public event Action OnDeathEvent;
    
    public UnitType type = UnitType.Archer;
    
    public float maxHitPoints = 10;
    public float hitPoints = 10;
    public int speed = 5;
    public float attackRate = 1f;
    public float damage = 1f;
    public float attackRange = 1f;
    public float visionRange = 5f;
    public bool isAlive = true;
    public bool immortalDebug;

    [HideInInspector] public NavMeshAgent agent;
    
    public Units target = null;

    public Team team = Team.TeamA;
    [HideInInspector] public Team targetTeam;
    
    public Animator animator;
    public GameObject healthCanvas;
    
    public abstract void Attack();
    public abstract void Hurt(float damageTaken);
    public abstract void Death();

    private void Awake()
    {
        //valueToChange = (valueToCheck) ? if true : else
        targetTeam = team == Team.TeamA ? Team.TeamB : Team.TeamA;
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 300;
        agent.acceleration = 300;
        hitPoints = maxHitPoints;
    }

    private void Start()
    {
        GetClosestEnemy();
    }
    
    public void GetClosestEnemy()
    {
        Units nearestUnit = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Units u in GameManager.Instance.AllUnits)
        {
            if (u.team == team || !u.isAlive) continue;
            float dist = Vector3.Distance(u.transform.position, currentPos);
            if (dist < minDist)
            {
                nearestUnit = u;
                minDist = dist;
            }
        }

        target = nearestUnit;
        
        if (target != null) target.OnDeathEvent += OnTargetDeath;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void InvokeDeathEvent()
    {
        OnDeathEvent?.Invoke();
    }

    private void OnTargetDeath()
    {
        target.OnDeathEvent -= OnTargetDeath;
        GetClosestEnemy();
        
    }

    public enum UnitType
    {
        Dummy,
        Archer,
        Sword,
        Healer
    }

    public enum Team
    {
        TeamA,
        TeamB
    }
}
