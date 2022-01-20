using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Killable : MonoBehaviour
{
    [SerializeField] private List<ProjectileSize> allowedProjectileSizes = new List<ProjectileSize>{ ProjectileSize.Small, ProjectileSize.Big };

    public bool CanBeDestroyedByProjectile(ProjectileSize size)
    {
        return allowedProjectileSizes.Contains(size);
    }
}
