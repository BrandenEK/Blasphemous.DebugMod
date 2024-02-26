using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

internal class HitboxData
{
    private readonly LineRenderer _line;

    public HitboxData(Collider2D collider)
    {
        // Create object as child of collider
        var obj = new GameObject("Hitbox");
        obj.transform.parent = collider.transform;
        obj.transform.localPosition = Vector3.zero;

        // Add line renderer component
        _line = obj.AddComponent<LineRenderer>();
        _line.material = Material;
        _line.sortingLayerName = "Foreground Paralax 3";
        _line.useWorldSpace = false;
        _line.startWidth = LINE_WIDTH;
        _line.endWidth = LINE_WIDTH;

        // Set up drawing based on collider type
        switch (collider)
        {
            case BoxCollider2D box:
                _line.DisplayBox(box);
                break;
            case CircleCollider2D circle:
                _line.DisplayCircle(circle);
                break;
            case CapsuleCollider2D capsule:
                _line.DisplayCapsule(capsule);
                break;
            case PolygonCollider2D polygon:
                _line.DisplayPolygon(polygon);
                break;
            default:
                return;
        }

        // Change color and order based on hitbox type
        Color color;
        int order;
        switch (collider.GetHitboxType())
        {
            case HitboxType.Hazard:
                ColorUtility.TryParseHtmlString("#FF007F", out color);
                order = 420;
                break;
            case HitboxType.Damageable:
                ColorUtility.TryParseHtmlString("#FFA500", out color);
                order = 400;
                break;
            case HitboxType.Player:
                ColorUtility.TryParseHtmlString("#00CCCC", out color);
                order = 380;
                break;
            case HitboxType.Sensor:
                ColorUtility.TryParseHtmlString("#660066", out color);
                order = 360;
                break;
            case HitboxType.Enemy:
                ColorUtility.TryParseHtmlString("#DD0000", out color);
                order = 340;
                break;
            case HitboxType.Interactable:
                ColorUtility.TryParseHtmlString("#FFFF33", out color);
                order = 320;
                break;
            case HitboxType.Trigger:
                ColorUtility.TryParseHtmlString("#0066CC", out color);
                order = 300;
                break;
            case HitboxType.Geometry:
                ColorUtility.TryParseHtmlString("#00CC00", out color);
                order = 100;
                break;
            case HitboxType.Other:
                ColorUtility.TryParseHtmlString("#000099", out color);
                order = 260;
                break;
            default:
                throw new System.Exception("Invalid hitbox type!");
        }

        _line.sortingOrder = order;
        _line.startColor = color;
        _line.endColor = color;
    }

    public void DestroyHitbox()
    {
        if (_line != null && _line.gameObject != null)
            Object.Destroy(_line.gameObject);
    }

    private static Material m_material;
    public static Material Material
    {
        get
        {
            if (m_material == null)
            {
                var obj = new GameObject("Temp");
                m_material = obj.AddComponent<SpriteRenderer>().material;
                Object.Destroy(obj);
            }
            return m_material;
        }
    }

    private const float LINE_WIDTH = 0.06f;
}
