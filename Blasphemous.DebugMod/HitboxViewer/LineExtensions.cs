using System.Linq;
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
        Vector2 offset = collider.offset;
        if (collider.transform.localScale.x < 0)
            offset.x *= -1;

        line.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            line.SetPosition(i, offset + points[i]);
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

    /// <summary>
    /// Draws outline for a CapsuleCollider2D
    /// </summary>
    public static void DisplayCapsule(this LineRenderer line, CapsuleCollider2D collider)
    {
        line.SetPoints(collider, new Vector2[CAPSULE_VERTICES + 1].AddCapsule(collider.size / 2));
    }

    /// <summary>
    /// Draws outline for a PolygonCollider2D
    /// </summary>
    public static void DisplayPolygon(this LineRenderer line, PolygonCollider2D collider)
    {
        // Skip empty polygons
        if (collider.pathCount == 0)
            return;

        Vector2[] points = collider.GetPath(0);
        if (points.Length > 0)
            points = points.Concat(new Vector2[] { points[0] }).ToArray();

        line.SetPoints(collider, points);
    }

    private const int BOX_VERTICES = 80;
    private const int CIRCLE_VERTICES = 80;
    private const int CAPSULE_VERTICES = 80;
}
