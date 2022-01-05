using System;
using UnityEngine;

public class Triggerable : MonoBehaviour
{
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
