
namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Types of hitboxes
/// </summary>
public enum HitboxType
{
    /// <summary> Invalid type </summary>
    Invalid,

    /// <summary> Currently disabled </summary>
    Inactive,
    /// <summary> Part of the level geometry </summary>
    Geometry,
    /// <summary> Any other hitbox </summary>
    Other,
    /// <summary> Has the isTrigger flag </summary>
    Trigger,
    /// <summary> Can be interacted with </summary>
    Interactable,
    /// <summary> Part of an enemy object </summary>
    Enemy,
    /// <summary> Detects when something enters its area </summary>
    Sensor,
    /// <summary> Part of the player object </summary>
    Player,
    /// <summary> Can be damaged </summary>
    Damageable,
    /// <summary> Damages the player on contact </summary>
    Hazard,
}
