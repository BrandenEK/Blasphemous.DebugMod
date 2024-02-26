
namespace Blasphemous.DebugMod.HitboxViewer;

internal enum HitboxType
{
    Hazard,
    Damageable,
    Player,
    Sensor,
    Enemy,
    Interactable,
    Trigger,
    Geometry,
    Other,

    Invalid
}

internal enum ColliderType
{
    Box,
    Circle,
    Capsule,
    Polygon,

    Invalid
}
