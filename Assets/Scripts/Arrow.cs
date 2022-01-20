using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Arrow : Triggerable
{
    public static float shootForce = 20;
    public float destructionTime = 0.5f;
    public enum Mode
    {
        Move = 0,
        Pinned,
        WaitToDestruct
    }

    public Mode mode;
    private float? _timeToDestruct;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        mode = Mode.Move;
        _timeToDestruct = Time.time + 1.5f;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(mode);
        if (mode == Mode.Move)
        {
            Debug.Log("move");
            //transform.position += transform.forward * shootForce;
            transform.Translate(Vector3.right * shootForce * Time.deltaTime);
        }

        if (mode == Mode.Pinned)
        {
            _timeToDestruct = Time.time + destructionTime;
            shootForce = 0.0f;
            mode = Mode.WaitToDestruct;

        }


        if (_timeToDestruct <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        if (mode != Mode.Move)
        {
            base.OnTrigger(other, hero);
            hero.playerController.Respawn();
        }
    }
}