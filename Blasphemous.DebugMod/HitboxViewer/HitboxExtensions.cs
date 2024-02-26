
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public static class HitboxExtensions
{
    // BoxCollider2D
    public static void DisplayBox(this LineRenderer renderer, BoxCollider2D collider)
    {
        // Skip colliders that are too large
        if (collider.size.x >= 15 || collider.size.y >= 15 || collider.size.x <= 0.1f || collider.size.y <= 0.1f)
            return;

        Vector2 halfSize = collider.size / 2f;
        Vector2 topLeft = new(-halfSize.x, halfSize.y);
        Vector2 topMiddle = new(0, halfSize.y);
        Vector2 topRight = halfSize;
        Vector2 middleRight = new(halfSize.x, 0);
        Vector2 bottomRight = new(halfSize.x, -halfSize.y);
        Vector2 bottomLeft = -halfSize;

        Vector2[] points = new Vector2[100];
        AddLine(points, topLeft, topRight, 0, 25);
        AddLine(points, topRight, bottomRight, 25, 25);
        AddLine(points, bottomRight, bottomLeft, 50, 25);
        AddLine(points, bottomLeft, topLeft, 75, 25);

        //Vector2[] points =
        //[
        //    topLeft,
        //    //new(0, halfSize.y),
        //    topRight,
        //    //new(halfSize.x, 0),
        //    bottomRight,
        //    //new(0, -halfSize.y),
        //    bottomLeft,
        //    topLeft,
        //    //new(-halfSize.x, 0),
        //    //, topLeft//, topRight, bottomRight, bottomLeft, topLeft
        //];

        //renderer.positionCount = 2;
        renderer.positionCount = points.Length;
        //renderer.SetPositions(points);
        for (int i = 0; i < points.Length; i++)
        {
            renderer.SetPosition(i, collider.offset + points[i]);
        }

        static void AddLine(Vector2[] points, Vector2 start, Vector2 end, int index, int count)
        {
            Vector2 diff = (end - start) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                Vector2 point = start + (diff * i);
                points[index + i] = point;
            }
        }
    }
}
