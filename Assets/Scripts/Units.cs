using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Units : MonoBehaviour
{
    public event Action OnDeathEvent;
    public static event Action OnDeathStaticEvent;
    
    public UnitType type = UnitType.Archer;
    
    public float maxHitPoints = 10;
    public float hitPoints = 10;
    public int speed = 5;
    public float attackRate = 1f;
    public float healRate = 0f;
    public float damage = 1f;
    public float critChance = 0.1f;
    public float critMultiplier = 2f;
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
    public ParticleSystem healAura;

    public int killCount;
    
    public abstract void Attack();
    public abstract void Hurt(float damageTaken, Units killer);
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
        GetClosestUnit();
    }

    private void GetClosestUnit()
    {
        Units nearestUnit = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Units target in GameManager.Instance.AllUnits)
        {
            if (!target.isAlive) continue; 
            if (target.team == team) continue;
            
            // if (isLookForEnemy) if (target.team == team) continue; //skip if kakampi, look for kalaban
            // if (!isLookForEnemy) if (target.team != team) continue; //skip if kalaban, look for kakampi
            
            float dist = Vector3.Distance(target.transform.position, currentPos);
            if (dist < minDist)
            {
                nearestUnit = target;
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
        OnDeathStaticEvent?.Invoke();
    }

    private void OnTargetDeath()
    {
        target.OnDeathEvent -= OnTargetDeath;
        GetClosestUnit();
        
    }
    
    public void Heal(float healTaken)
    {
        if (!isAlive) return;
        
        hitPoints += healTaken;
        healthCanvas.transform.GetChild(1).GetComponent<Image>().fillAmount = hitPoints / maxHitPoints;
        
        healAura.Play();
        if (hitPoints >= maxHitPoints)
        {
            hitPoints = maxHitPoints;
        }
    }
    
    public float CalculateDamage()
    {
        float damageToApply = damage;

        if (Random.Range(0f, 1f) <= critChance)
        {
            float critMultiplierChance = Random.Range(0.05f, 1f);
            float critMultiplierToApply = critMultiplier * critMultiplierChance;
            damageToApply *= critMultiplierToApply;
        }

        return damageToApply;
    }

    public void IncreaseKillCount()
    {
        killCount++;
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
