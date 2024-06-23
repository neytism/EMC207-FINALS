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
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        
        if (distanceToPlayer <= attackRange)
        {
            target.Hurt(damage);
        }
    }

    public void PlaySlash()
    {
        _slash.Play();
    }

    public override void Hurt(float damageTaken)
    {
        if (!isAlive) return;
        
        healthCanvas.SetActive(true);
        if (!immortalDebug) hitPoints -= damageTaken;
        healthCanvas.transform.GetChild(1).GetComponent<Image>().fillAmount = hitPoints / maxHitPoints;

        if (hitPoints <= 0)
        {
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
