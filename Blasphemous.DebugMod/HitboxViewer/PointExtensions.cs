using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Extensions for calculating points for line renderers
/// </summary>
public static class PointExtensions
{
    /// <summary>
    /// Calculates the points for a box
    /// </summary>
    public static Vector2[] AddBox(this Vector2[] points, Vector2 size)
    {
        if (points.Length % 4 != 0)
            throw new System.ArgumentException("Points length must be divisible by 4");

        int quad = points.Length / 4;
        Vector2 half = size / 2;

        Vector2 topLeft = new(-half.x, half.y);
        Vector2 topRight = half;
        Vector2 bottomRight = new(half.x, -half.y);
        Vector2 bottomLeft = -half;

        return points
            .AddLine(topLeft, topRight, quad * 0, quad)
            .AddLine(topRight, bottomRight, quad * 1, quad)
            .AddLine(bottomRight, bottomLeft, quad * 2, quad)
            .AddLine(bottomLeft, topLeft, quad * 3, quad);
    }

    /// <summary>
    /// Calculates the points for a line of a box
    /// </summary>
    private static Vector2[] AddLine(this Vector2[] points, Vector2 start, Vector2 end, int index, int count)
    {
        Vector2 diff = (end - start) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 point = start + (diff * i);
            points[index + i] = point;
        }
        return points;
    }

    /// <summary>
    /// Calculates the points for a circle
    /// </summary>
    public static Vector2[] AddCircle(this Vector2[] points, float radius)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float circumferenceProgress = (float)i / (points.Length - 1);

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            points[i] = new Vector2(radius * xScaled, radius * yScaled);
        }
        return points;
    }

    /// <summary>
    /// Calculates the points for a capsule
    /// </summary>
    public static Vector2[] AddCapsule(this Vector2[] points, Vector2 radius)
    {
        float currAngle = 20f;

        for (int i = 0; i < points.Length; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * currAngle) * radius.x;
            float y = Mathf.Cos(Mathf.Deg2Rad * currAngle) * radius.y;

            points[i] = new Vector2(x, y);
            currAngle += (360f / points.Length);
        }
        return points;
    }
}
