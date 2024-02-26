using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Tools.Level;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

internal static class ColliderExtensions
{
    public static HitboxType GetHitboxType(this Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Trap"))
        {
            return HitboxType.Hazard;
        }
        if (collider.transform.HasComponentInParent<DamageArea>())
        {
            return HitboxType.Damageable;
        }
        if (collider.transform.HasComponentInParent<Penitent>())
        {
            return HitboxType.Player;
        }
        if (collider.transform.HasComponentInParent<EnemySensor>())
        {
            return HitboxType.Sensor;
        }
        if (collider.transform.HasComponentInParent<Enemy>())
        {
            return HitboxType.Enemy;
        }
        if (collider.transform.HasComponentInParent<Interactable>())
        {
            return HitboxType.Interactable;
        }
        if (collider.isTrigger)
        {
            return HitboxType.Trigger;
        }
        if (collider.name.StartsWith("GEO_"))
        {
            return HitboxType.Geometry;
        }

        return HitboxType.Other;
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
