using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Arrow : Triggerable
{

    public float shootForce = 20;
    public float destructionTime = 1f;

    public bool pinnedOnFloor = false;
    private float? _timeToDestruct;
    
    // Start is called before the first frame update
    void Start()
    {
        _timeToDestruct = Time.time + 3.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (shootForce > 0.1f){
            transform.position += transform.forward * shootForce;
        }
        if (pinnedOnFloor)
        {
            _timeToDestruct = Time.time + destructionTime;
            shootForce = 0.0f;
        }
        

        if (_timeToDestruct <= Time.time)
        {
            Destroy(gameObject);
        }

    }

    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        if(!pinnedOnFloor)
        {
            base.OnTrigger(other, hero);
            hero.playerController.Respawn();
        }
    }

}
