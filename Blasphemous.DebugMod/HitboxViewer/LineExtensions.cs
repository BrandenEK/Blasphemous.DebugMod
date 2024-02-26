using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Extensions for setting up line renderers
/// </summary>
public static class LineExtensions
{
    /// <summary>
    /// Sets the points for a line renderer
    /// </summary>
    public static void SetPoints(this LineRenderer line, Collider2D collider, Vector2[] points)
    {
        line.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            line.SetPosition(i, collider.offset + points[i]);
        }
    }

    /// <summary>
    /// Draws outline for a BoxCollider2D
    /// </summary>
    public static void DisplayBox(this LineRenderer line, BoxCollider2D collider)
    {
        // Skip colliders that are too large
        if (collider.size.x >= 15 || collider.size.y >= 15 || collider.size.x <= 0.1f || collider.size.y <= 0.1f)
            return;

        line.SetPoints(collider, new Vector2[BOX_VERTICES].AddBox(collider.size));
    }

    /// <summary>
    /// Draws outline for a CircleCollider2D
    /// </summary>
    public static void DisplayCircle(this LineRenderer line, CircleCollider2D collider)
    {
        // Skip colliders that are too large
        if (collider.radius >= 5 || collider.radius <= 0.1f)
            return;

        line.SetPoints(collider, new Vector2[CIRCLE_VERTICES].AddCircle(collider.radius));
    }

    private const int BOX_VERTICES = 80;
    private const int CIRCLE_VERTICES = 80;
}
