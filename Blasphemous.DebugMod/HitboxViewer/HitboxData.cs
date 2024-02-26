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
        Color color = Color.green;
        int order = 10000;
        switch (collider.GetHitboxType())
        {
            //case HitboxType.Hazard:
            //    color = Main.DebugMod.DebugSettings.hazardColor;
            //    order = 420;
            //    break;
            //case HitboxType.Damageable:
            //    color = Main.DebugMod.DebugSettings.damageableColor;
            //    order = 400;
            //    break;
            //case HitboxType.Player:
            //    color = Main.DebugMod.DebugSettings.playerColor;
            //    order = 380;
            //    break;
            //case HitboxType.Sensor:
            //    color = Main.DebugMod.DebugSettings.sensorColor;
            //    order = 360;
            //    break;
            //case HitboxType.Enemy:
            //    color = Main.DebugMod.DebugSettings.enemyColor;
            //    order = 340;
            //    break;
            //case HitboxType.Interactable:
            //    color = Main.DebugMod.DebugSettings.interactableColor;
            //    order = 320;
            //    break;
            //case HitboxType.Trigger:
            //    color = Main.DebugMod.DebugSettings.triggerColor;
            //    order = 300;
            //    break;
            //case HitboxType.Geometry:
            //    color = Main.DebugMod.DebugSettings.geometryColor;
            //    order = 100;
            //    break;
            //case HitboxType.Other:
            //    color = Main.DebugMod.DebugSettings.otherColor;
            //    order = 260;
            //    break;
            //default:
            //    throw new System.Exception("A valid type should be calculated before now!");
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
