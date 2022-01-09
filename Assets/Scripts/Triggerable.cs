using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Triggerable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var hero = other.gameObject.GetComponent<Hero>();
        if (hero == null) return;
        OnTrigger(other, hero);
    }

    protected virtual void OnTrigger(Collider2D other, Hero hero)
    {
    }
}
