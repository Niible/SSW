using System;
using UnityEngine;

public class Triggerable : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var hero = other.gameObject.GetComponent<Hero>();
        if (hero == null) return;
        OnTrigger(hero);
    }

    protected virtual void OnTrigger(Hero hero)
    {
    }
}
