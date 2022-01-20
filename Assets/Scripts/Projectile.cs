using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileSize
{
    Small = 0,
    Big
}

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] public ProjectileSize size;
    [SerializeField] public Vector2 direction;
    
    private const float _SMALL_PROJECTILE_SPEED = 15f;
    private const float _BIG_PROJECTILE_SPEED = 10f;
    private const float _DURATION_IN_SECONDS = 3f;
    
    private Rigidbody2D _rigidbody2D;
    private float _duration = 0f;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        _duration += Time.deltaTime;
        if(_duration >= _DURATION_IN_SECONDS)
        {
            Destroy(gameObject);
        }
        if (size == ProjectileSize.Small)
        {
            _rigidbody2D.velocity = direction * _SMALL_PROJECTILE_SPEED;
        }
        else
        {
            _rigidbody2D.velocity = direction * _BIG_PROJECTILE_SPEED;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        CheckAndDestroyKillableObject(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        CheckAndDestroyKillableObject(col);
    }

    private void CheckAndDestroyKillableObject(Component collider2D)
    {
        var killableComponent = collider2D.gameObject.GetComponent<Killable>();
        
        if (killableComponent != null && killableComponent.CanBeDestroyedByProjectile(size))
        {
            Destroy(killableComponent.gameObject);
        }
        Destroy(gameObject);
    }
}
