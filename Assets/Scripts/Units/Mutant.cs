using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Mutant : Units
{
    //TODO: fix attack rate 

    private float attackTime;
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
}