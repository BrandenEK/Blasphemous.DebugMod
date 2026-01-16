
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
}

public class ColliderSettings
{
    public string Color { get; set; }

    public bool Visible { get; set; }

    public ColliderSettings(string color, bool visible)
    {
        Color = color;
        Visible = visible;
    }
}
