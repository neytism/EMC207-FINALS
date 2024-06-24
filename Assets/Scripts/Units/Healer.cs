using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Healer : Units
{
    
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;
    
    [SerializeField] private ParticleSystem _healCircle;
    [SerializeField] private float _healValue = 5f;
    
    private float _healTime;
    private int _prevKillCount = 0;

    private void Update()
    {
        if (GameManager.Instance.battleDone) return;
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

    public override void Attack()
    {
        foreach (Units unit in GameManager.Instance.AllUnits)
        {
            if (unit.team == team)
            {
                float dist = Vector3.Distance(unit.transform.position, transform.position);
                
                if (dist <= visionRange)
                {
                    unit.Heal(_healValue);
                }
            }
        }
    }

    public void Shoot()
    {
        GameObject obj = ObjectPool.Instance.PoolObject(_projectilePrefab, _firePoint.position);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.SetTargetTeam = targetTeam;
        projectile.SetDamage = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
    }

    public void PlayHeal()
    {
        if (GameManager.Instance.battleDone) return;
        _healCircle.Play();
    }

    public override void Hurt(float damageTaken, Units killer)
    {
        if (!isAlive) return;
        
        healthCanvas.SetActive(true);
        if (!immortalDebug) hitPoints -= damageTaken;
        healthCanvas.transform.GetChild(1).GetComponent<Image>().fillAmount = hitPoints / maxHitPoints;

        if (hitPoints <= 0)
        {
            killer.IncreaseKillCount();
            Death();
        }
    }

    public override void Death()
    {
        isAlive = false;
        InvokeDeathEvent();
        animator.SetTrigger("Death");
        healthCanvas.SetActive(false);
        agent.speed = 0;
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
       
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
}