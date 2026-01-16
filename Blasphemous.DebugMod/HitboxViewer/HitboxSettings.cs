using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class HitboxSettings
{
    public ColliderSettings Inactive { get; set; }

    public ColliderSettings Hazard { get; set; }

    public ColliderSettings Damageable { get; set; }

    public ColliderSettings Player { get; set; }

    public ColliderSettings Sensor { get; set; }

    public ColliderSettings Enemy { get; set; }

    public ColliderSettings Interactable { get; set; }

    public ColliderSettings Geometry { get; set; }

    public ColliderSettings Trigger { get; set; }

    public ColliderSettings Other { get; set; }

    public ColliderSettings GetColliderSettings(HitboxType type)
    {
        return type switch
        {
            HitboxType.Inactive => Inactive,
            HitboxType.Hazard => Hazard,
            HitboxType.Damageable => Damageable,
            HitboxType.Player => Player,
            HitboxType.Sensor => Sensor,
            HitboxType.Enemy => Enemy,
            HitboxType.Interactable => Interactable,
            HitboxType.Trigger => Trigger,
            HitboxType.Geometry => Geometry,
            HitboxType.Other => Other,
            _ => throw new System.Exception($"Invalid hitbox type: {type}"),
        };
    }
}

public class ColliderSettings
{
    public Color Color { get; set; }

    public bool Visible { get; set; }

    public ColliderSettings(string color, bool visible)
    {
        Color = ColorUtility.TryParseHtmlString(color, out Color c) ? c : Color.white;
        Visible = visible;
    }
}
