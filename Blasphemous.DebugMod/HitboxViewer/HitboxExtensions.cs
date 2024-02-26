
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public static class HitboxExtensions
{
    public static void SetPoints(this LineRenderer line, Collider2D collider, Vector2[] points)
    {
        line.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            line.SetPosition(i, collider.offset + points[i]);
        }
    }

    // Colliders

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

    private const int BOX_VERTICES = 80;

    // Points

    internal static Vector2[] AddBox(this Vector2[] points, Vector2 size)
    {
        if (points.Length % 4 != 0)
            throw new System.ArgumentException("Points length must be divisible by 4");

        Vector2 half = size / 2;
        Vector2 topLeft = new(-half.x, half.y);
        Vector2 topRight = half;
        Vector2 bottomRight = new(half.x, -half.y);
        Vector2 bottomLeft = -half;
        int quad = points.Length / 4;

        return points
            .AddLine(topLeft, topRight, quad * 0, quad)
            .AddLine(topRight, bottomRight, quad * 1, quad)
            .AddLine(bottomRight, bottomLeft, quad * 2, quad)
            .AddLine(bottomLeft, topLeft, quad * 3, quad);
    }

    internal static Vector2[] AddLine(this Vector2[] points, Vector2 start, Vector2 end, int index, int count)
    {
        Vector2 diff = (end - start) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 point = start + (diff * i);
            points[index + i] = point;
        }
        return points;
    }
}
