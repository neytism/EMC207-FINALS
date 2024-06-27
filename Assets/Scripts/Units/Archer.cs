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
        projectile.SetValueEffect = CalculateDamage();
        projectile.SetSpeed = _arrowSpeed;
        projectile.SetDirection = transform.forward;
        projectile.SetUnit = this;
        obj.SetActive(true);
    }
    
    public override void OnStartBattle()
    {
       
    }
    
    public override void OnDeath()
    {
        
    }
    
    public override void OnHurt()
    {
        
    }
}
