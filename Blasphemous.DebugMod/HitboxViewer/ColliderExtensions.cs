using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

internal static class ColliderExtensions
{
    public static HitboxType GetHitboxType(this Collider2D collider)
    {
        //if (collider.transform.GetComponent<AttackHit>() != null)
        //{
        //    return HitboxType.Hazard;
        //}
        //if (collider.transform.HasComponentInParent<CollisionsCallback>())
        //{
        //    return HitboxType.Damageable;
        //}
        //if (collider.transform.HasComponentInParent<PlayerPersistentComponent>())
        //{
        //    return HitboxType.Player;
        //}
        //if (collider.transform.HasComponentInParent<TriggersCallback>())
        //{
        //    return HitboxType.Sensor;
        //}
        //if (collider.transform.HasComponentInParent<AliveEntity>())
        //{
        //    return HitboxType.Enemy;
        //}
        //if (collider.transform.HasComponentInParent<IInteractable>())
        //{
        //    return HitboxType.Interactable;
        //}
        //if (collider.isTrigger)
        //{
        //    return HitboxType.Trigger;
        //}
        //if (collider.name.StartsWith("GEO_"))
        //{
        //    return HitboxType.Geometry;
        //}

        return HitboxType.Other;
    }

    public static ColliderType GetColliderType(this Collider2D collider)
    {
        return collider switch
        {
            BoxCollider2D => ColliderType.Box,
            CircleCollider2D => ColliderType.Circle,
            CapsuleCollider2D => ColliderType.Capsule,
            PolygonCollider2D => ColliderType.Polygon,
            _ => ColliderType.Invalid,
        };
    }

    public static bool HasComponentInParent<T>(this Transform transform)
    {
        Transform parent = transform;
        while (parent != null)
        {
            if (parent.GetComponent<T>() != null)
                return true;
            parent = parent.parent;
        }

        return false;
    }
}
