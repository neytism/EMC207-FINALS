using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Mutant : Units
{
    //TODO: fix attack rate 

    private float attackTime;

    private void Start()
    {
    }

    public override void OnStartBattle()
    {
       
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


    public override void OnHurt()
    {
        
    }
    
    public override void OnDeath()
    {
        
    }
    
}