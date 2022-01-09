using UnityEngine;

public class RespawnPoint : Triggerable
{
    [SerializeField] public int facingDirection = 1;
    [SerializeField] public CameraPosition cameraPosition;
    
    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        base.OnTrigger(other, hero);
        hero.SetLastRespawnPoint(this);
    }
}
