using UnityEngine;

public class InstantKillTrigger : Triggerable
{
    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        base.OnTrigger(other, hero);
        hero.playerController.Respawn();
    }
}
