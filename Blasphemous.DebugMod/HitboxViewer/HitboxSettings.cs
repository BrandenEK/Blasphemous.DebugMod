using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class HitboxSettings
{
    private readonly Dictionary<HitboxType, ColliderSettings> _settings = [];

    public HitboxSettings(params ColliderSettings[] colliders)
    {
        foreach (var collider in colliders)
            _settings.Add(collider.Type, collider);
    }

    public ColliderSettings this[HitboxType type]
    {
        get => _settings[type];
    }

    public IEnumerable<ColliderSettings> AllColliders => _settings.Values;
}

public class ColliderSettings
{
    public HitboxType Type { get; set; }

    public Color Color { get; set; }

    public bool Visible { get; set; }

    public ColliderSettings(HitboxType type, string color, bool visible)
    {
        Type = type;
        Color = ColorUtility.TryParseHtmlString(color, out Color c) ? c : Color.white;
        Visible = visible;
    }
}
