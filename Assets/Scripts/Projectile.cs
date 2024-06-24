using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffect;
    
    private Units _shooter;
    private float _damage;
    private float _speed;
    private Vector3 _direction;
    private Units.Team _targetTeam;
    private Collider _col;

    public Units SetUnit
    {
        set => _shooter = value;
    }
    public float SetDamage
    {
        set => _damage = value;
    }
    
    public float SetSpeed
    {
        set => _speed = value;
    }
    
    public Vector3 SetDirection
    {
        set => _direction = value;
    }
    
    public Units.Team SetTargetTeam
    {
        set => _targetTeam = value;
    }

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _col.enabled = true;
        StartCoroutine(LifeTime());
    }

    private void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Units unit = other.GetComponent<Units>();
        
        if (unit != null)
        {
            if (unit.team == _targetTeam)
            {
                unit.Hurt(_damage, _shooter);
                StopCoroutine(LifeTime());
                StartCoroutine(OnHit());
                
            }
        }
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
    
    IEnumerator OnHit()
    {
        _hitEffect.Play();
        _col.enabled = false;
        _speed = 0;
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
}
