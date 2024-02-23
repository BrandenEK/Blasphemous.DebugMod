using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxViewer(Sprite image) : BaseModule("Hitbox_Viewer", false)
{
    private readonly List<GameObject> _sceneHitboxes = new();
    private readonly Sprite _image = image;

    protected override void OnActivate()
    {
        GameObject baseHitbox = CreateBaseHitbox();
        _sceneHitboxes.Clear();

        foreach (BoxCollider2D collider in Object.FindObjectsOfType<BoxCollider2D>())
        {
            if (collider.name.StartsWith(GEOMETRY_NAME))
                continue;

            GameObject hitbox = Object.Instantiate(baseHitbox, collider.transform);
            hitbox.transform.localPosition = Vector3.zero;

            Transform side = hitbox.transform.GetChild(0);
            side.localPosition = new Vector3(collider.offset.x, collider.size.y / 2 + collider.offset.y, 0);
            side.localScale = new Vector3(collider.size.x, SCALE_AMOUNT / collider.transform.localScale.y, 0);

            side = hitbox.transform.GetChild(1);
            side.localPosition = new Vector3(-collider.size.x / 2 + collider.offset.x, collider.offset.y, 0);
            side.localScale = new Vector3(SCALE_AMOUNT / collider.transform.localScale.x, collider.size.y, 0);

            side = hitbox.transform.GetChild(2);
            side.localPosition = new Vector3(collider.size.x / 2 + collider.offset.x, collider.offset.y, 0);
            side.localScale = new Vector3(SCALE_AMOUNT / collider.transform.localScale.x, collider.size.y, 0);

            side = hitbox.transform.GetChild(3);
            side.localPosition = new Vector3(collider.offset.x, -collider.size.y / 2 + collider.offset.y, 0);
            side.localScale = new Vector3(collider.size.x, SCALE_AMOUNT / collider.transform.localScale.y, 0);

            _sceneHitboxes.Add(hitbox);
        }

        Object.Destroy(baseHitbox);
        Main.Debugger.Log($"Adding outlines to {_sceneHitboxes.Count} hitboxes");
    }

    protected override void OnDeactivate()
    {
        for (int i = 0; i < _sceneHitboxes.Count; i++)
        {
            if (_sceneHitboxes[i] != null)
                Object.Destroy(_sceneHitboxes[i]);
        }
        _sceneHitboxes.Clear();
    }

    private GameObject CreateBaseHitbox()
    {
        GameObject baseHitbox = new GameObject("Hitbox");
        GameObject side = new GameObject("TOP");
        side.AddComponent<SpriteRenderer>().sprite = _image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("LEFT");
        side.AddComponent<SpriteRenderer>().sprite = _image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("RIGHT");
        side.AddComponent<SpriteRenderer>().sprite = _image;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("BOTTOM");
        side.AddComponent<SpriteRenderer>().sprite = _image;
        side.transform.parent = baseHitbox.transform;
        return baseHitbox;
    }

    private const float SCALE_AMOUNT = 0.05f;
    private const string GEOMETRY_NAME = "GEO_Block";
}
