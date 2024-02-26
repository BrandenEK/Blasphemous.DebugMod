using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

internal class HitboxData
{
    private readonly LineRenderer _line;

    public HitboxData(Collider2D col, Sprite image)
    {
        if (col is not BoxCollider2D collider)
            return;

        // Create object as child of collider
        var obj = new GameObject("Hitbox");
        obj.transform.parent = collider.transform;
        obj.transform.localPosition = Vector3.zero;

        // Add line renderer component
        _line = obj.AddComponent<LineRenderer>();
        _line.material = Material;//new Material(Shader.Find("Default"));
        _line.sortingLayerName = "Foreground Paralax 3";
        _line.useWorldSpace = false;
        //_line.SetWidth(LINE_WIDTH, LINE_WIDTH);
        _line.startColor = Color.green;
        _line.endColor = Color.green;
        //Main.Debugger.Log("Corner: " + _line.numCornerVertices);
        //_line.numCornerVertices = 8;
        //_line.loop = true;
        _line.alignment = LineAlignment.View;

        //_line.SetColors(Color.green, Color.cyan);
        _line.startWidth = LINE_WIDTH;// * collider.size.x / 2;
        _line.endWidth = LINE_WIDTH;// * collider.size.x / 2;
        _line.sortingOrder = 10000000;

        _line.DisplayBox(collider);

        //GameObject baseHitbox = CreateBaseHitbox(image);

        //GameObject hitbox = Object.Instantiate(baseHitbox, collider.transform);
        //hitbox.transform.localPosition = Vector3.zero;

        //Transform side = hitbox.transform.GetChild(0);
        //side.localPosition = new Vector3(collider.offset.x, collider.size.y / 2 + collider.offset.y, 0);
        //side.localScale = new Vector3(collider.size.x, SCALE_AMOUNT / collider.transform.localScale.y, 0);

        //side = hitbox.transform.GetChild(1);
        //side.localPosition = new Vector3(-collider.size.x / 2 + collider.offset.x, collider.offset.y, 0);
        //side.localScale = new Vector3(SCALE_AMOUNT / collider.transform.localScale.x, collider.size.y, 0);

        //side = hitbox.transform.GetChild(2);
        //side.localPosition = new Vector3(collider.size.x / 2 + collider.offset.x, collider.offset.y, 0);
        //side.localScale = new Vector3(SCALE_AMOUNT / collider.transform.localScale.x, collider.size.y, 0);

        //side = hitbox.transform.GetChild(3);
        //side.localPosition = new Vector3(collider.offset.x, -collider.size.y / 2 + collider.offset.y, 0);
        //side.localScale = new Vector3(collider.size.x, SCALE_AMOUNT / collider.transform.localScale.y, 0);

        //_outlineObject = hitbox;
        //Object.Destroy(baseHitbox);
    }

    private GameObject CreateBaseHitbox(Sprite image)
    {
        GameObject baseHitbox = new GameObject("Hitbox");
        GameObject side = new GameObject("TOP");
        side.AddComponent<SpriteRenderer>().sprite = image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("LEFT");
        side.AddComponent<SpriteRenderer>().sprite = image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("RIGHT");
        side.AddComponent<SpriteRenderer>().sprite = image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("BOTTOM");
        side.AddComponent<SpriteRenderer>().sprite = image;
        side.transform.parent = baseHitbox.transform;
        return baseHitbox;
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

    private const float SCALE_AMOUNT = 0.05f;
    private const float LINE_WIDTH = 0.06f;

    //private readonly LineRenderer _line;

    //public HitboxData(Collider2D collider)
    //{
    //    ColliderType colliderType = collider.GetColliderType();
    //    HitboxType hitboxType = collider.GetHitboxType();

    //    // Verify that the collider type should be shown
    //    if (colliderType == ColliderType.Invalid)
    //        return;

    //    // Verify that the hitbox type should be shown
    //    if (!Main.DebugMod.HitboxViewer.ToggledHitboxes[hitboxType])
    //        return;

    //    // Create object as child of collider
    //    var obj = new GameObject("Hitbox");
    //    obj.transform.parent = collider.transform;
    //    obj.transform.localPosition = Vector3.zero;

    //    // Add line renderer component
    //    _line = obj.AddComponent<LineRenderer>();
    //    _line.material = Main.DebugMod.HitboxViewer.HitboxMaterial;
    //    _line.sortingLayerName = "Foreground Parallax 2";
    //    _line.useWorldSpace = false;
    //    _line.SetWidth(LINE_WIDTH, LINE_WIDTH);

    //    // Set up drawing based on collider type
    //    switch (colliderType)
    //    {
    //        case ColliderType.Box:
    //            _line.DisplayBox(collider.Cast<BoxCollider2D>());
    //            break;
    //        case ColliderType.Circle:
    //            _line.DisplayCircle(collider.Cast<CircleCollider2D>());
    //            break;
    //        case ColliderType.Capsule:
    //            _line.DisplayCapsule(collider.Cast<CapsuleCollider2D>());
    //            break;
    //        case ColliderType.Polygon:
    //            _line.DisplayPolygon(collider.Cast<PolygonCollider2D>());
    //            break;
    //        default:
    //            throw new System.Exception("A valid type should be calculated before now!");
    //    }

    //    // Change color and order based on hitbox type
    //    Color color;
    //    int order;
    //    switch (hitboxType)
    //    {
    //        case HitboxType.Hazard:
    //            color = Main.DebugMod.DebugSettings.hazardColor;
    //            order = 420;
    //            break;
    //        case HitboxType.Damageable:
    //            color = Main.DebugMod.DebugSettings.damageableColor;
    //            order = 400;
    //            break;
    //        case HitboxType.Player:
    //            color = Main.DebugMod.DebugSettings.playerColor;
    //            order = 380;
    //            break;
    //        case HitboxType.Sensor:
    //            color = Main.DebugMod.DebugSettings.sensorColor;
    //            order = 360;
    //            break;
    //        case HitboxType.Enemy:
    //            color = Main.DebugMod.DebugSettings.enemyColor;
    //            order = 340;
    //            break;
    //        case HitboxType.Interactable:
    //            color = Main.DebugMod.DebugSettings.interactableColor;
    //            order = 320;
    //            break;
    //        case HitboxType.Trigger:
    //            color = Main.DebugMod.DebugSettings.triggerColor;
    //            order = 300;
    //            break;
    //        case HitboxType.Geometry:
    //            color = Main.DebugMod.DebugSettings.geometryColor;
    //            order = 100;
    //            break;
    //        case HitboxType.Other:
    //            color = Main.DebugMod.DebugSettings.otherColor;
    //            order = 260;
    //            break;
    //        default:
    //            throw new System.Exception("A valid type should be calculated before now!");
    //    }
    //    _line.SetColors(color, color);
    //    _line.sortingOrder = order;
    //}

    //public void DestroyHitbox()
    //{
    //    if (_line != null && _line.gameObject != null)
    //        Object.Destroy(_line.gameObject);
    //}

    //private const float LINE_WIDTH = 0.04f;
}
