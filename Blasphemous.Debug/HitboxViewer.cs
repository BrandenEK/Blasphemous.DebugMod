using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Debug;

/// <summary>
/// Displays hitboxes on all box colliders
/// </summary>
public class HitboxViewer : IModule
{
    private readonly List<GameObject> sceneHitboxes = new();
    private readonly Sprite hitboxImage;

    private bool _enabledHitboxes = false;
    /// <summary>
    /// Whether this module is active
    /// </summary>
    public bool EnabledHitboxes
    {
        get => _enabledHitboxes;
        set
        {
            _enabledHitboxes = value;
            if (value)
            {
                ShowHitboxes();
            }
            else
            {
                HideHitboxes();
            }
        }
    }

    internal HitboxViewer(Sprite hitbox)
    {
        hitboxImage = hitbox;
    }

    /// <summary>
    /// On load, show all hitboxes
    /// </summary>
    public void OnLevelLoaded()
    {
        if (EnabledHitboxes)
            ShowHitboxes();
    }

    /// <summary>
    /// On unload, hide all hitboxes
    /// </summary>
    public void OnLevelUnloaded()
    {
        HideHitboxes();
    }

    /// <summary>
    /// Every frame, check for input
    /// </summary>
    public void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown("Hitbox_Viewer"))
        {
            EnabledHitboxes = !EnabledHitboxes;
        }
    }

    private void ShowHitboxes()
    {
        GameObject baseHitbox = CreateBaseHitbox();
        sceneHitboxes.Clear();

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

            sceneHitboxes.Add(hitbox);
        }

        Object.Destroy(baseHitbox);
        Main.Debugger.Log($"Adding outlines to {sceneHitboxes.Count} hitboxes");
    }

    private void HideHitboxes()
    {
        for (int i = 0; i < sceneHitboxes.Count; i++)
        {
            if (sceneHitboxes[i] != null)
                Object.Destroy(sceneHitboxes[i]);
        }
        sceneHitboxes.Clear();
    }

    private GameObject CreateBaseHitbox()
    {
        GameObject baseHitbox = new GameObject("Hitbox");
        GameObject side = new GameObject("TOP");
        side.AddComponent<SpriteRenderer>().sprite = hitboxImage;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("LEFT");
        side.AddComponent<SpriteRenderer>().sprite = hitboxImage;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("RIGHT");
        side.AddComponent<SpriteRenderer>().sprite = hitboxImage;
        side.transform.parent = baseHitbox.transform;
        side = new GameObject("BOTTOM");
        side.AddComponent<SpriteRenderer>().sprite = hitboxImage;
        side.transform.parent = baseHitbox.transform;
        return baseHitbox;
    }

    private const float SCALE_AMOUNT = 0.05f;
    private const string GEOMETRY_NAME = "GEO_Block";
}
