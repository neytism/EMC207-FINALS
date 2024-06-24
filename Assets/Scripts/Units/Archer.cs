using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Archer : Units
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;

    public override void Attack()
    {
        GameObject obj = ObjectPool.Instance.PoolObject(_arrowPrefab, _firePoint.position);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.SetTargetTeam = targetTeam;
        projectile.SetDamage = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
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
