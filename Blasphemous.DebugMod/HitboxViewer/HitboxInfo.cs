using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Information regarding a Collider2D
/// </summary>
public readonly struct HitboxInfo(Collider2D collider, HitboxType type, bool isVisible)
{
    /// <summary>
    /// The actual collider
    /// </summary>
    public Collider2D Collider { get; } = collider;

    /// <summary>
    /// The type of hitbox
    /// </summary>
    public HitboxType Type { get; } = type;

    /// <summary>
    /// Whether this collider should be displayed
    /// </summary>
    public bool IsVisible { get; } = isVisible;
}
