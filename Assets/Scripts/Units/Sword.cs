using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Sword : Units
{
    
    [SerializeField] private ParticleSystem _slash;
    
    public override void Attack()
    {
        if (target == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        
        if (distanceToPlayer <= attackRange)
        {
            target.Hurt(CalculateDamage(), this);
        }
    }

    public override void OnHurt()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    public void PlaySlash()
    {
        _slash.Play();
    }
    
    public override void OnStartBattle()
    {
       
    }
    
}
