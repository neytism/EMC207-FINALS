using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffect;
    
    private Units _shooter;
    private float _valueEffect;
    private float _speed;
    private Vector3 _direction;
    private Collider _col;
    private Rigidbody _rb;

    public Units SetUnit
    {
        set => _shooter = value;
    }
    public float SetValueEffect
    {
        set => _valueEffect = value;
    }
    
    public float SetSpeed
    {
        set => _speed = value;
    }
    
    public Vector3 SetDirection
    {
        set => _direction = value;
    }

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _col.enabled = true;
        GetComponent<Collider>().enabled = true;
        StartCoroutine(LifeTime());
    }

    private void Update()
    {
        //transform.position += _direction * (_speed * Time.deltaTime);
        
        Vector3 desiredMovement = _direction * (_speed * Time.deltaTime);
        _rb.MovePosition(_rb.position + desiredMovement);
    }

    private void OnTriggerEnter(Collider other)
    {
        Units unit = other.GetComponent<Units>();
        
        if (unit != null)
        {
            if (unit.team == _shooter.targetTeam)
            {
                unit.Hurt(_valueEffect, _shooter);
                GetComponent<Collider>().enabled = false;
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
