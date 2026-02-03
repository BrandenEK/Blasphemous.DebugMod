using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxModule : BaseModule
{
    private readonly HitboxSettings _settings;

    private CameraComponent _cameraComponent;
    private RawImage _imageComponent;

    public HitboxModule() : base("Hitbox_Viewer", false)
    {
        _settings = new HitboxSettings(
            new ColliderSettings(HitboxType.Damageable, "#FFA500", true),
            new ColliderSettings(HitboxType.Enemy, "#DD0000", true),
            new ColliderSettings(HitboxType.Geometry, "#00CC00", true),
            new ColliderSettings(HitboxType.Hazard, "#FF007F", true),
            new ColliderSettings(HitboxType.Inactive, "#4D4E4C", true),
            new ColliderSettings(HitboxType.Interactable, "#FFFF33", true),
            new ColliderSettings(HitboxType.Other, "#000099", true),
            new ColliderSettings(HitboxType.Player, "#00CCCC", true),
            new ColliderSettings(HitboxType.Sensor, "#660066", true),
            new ColliderSettings(HitboxType.Trigger, "#0066CC", true));

        var comp = Main.Instance.gameObject.AddComponent<HitboxWindow>();
        comp.InjectSettings(_settings);
    }

    protected override void OnActivate()
    {
        if (_cameraComponent == null)
        {
            ModLog.Info("Creating new hitbox camera");
            var tex = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            tex.Create();

            var camObject = new GameObject("Hitbox Camera");
            camObject.transform.SetParent(Camera.main.transform, false);

            var camera = camObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = Camera.main.orthographicSize;
            camera.aspect = Camera.main.aspect;
            camera.depth = 10;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = new Color32(0, 0, 0, 0);
            camera.targetTexture = tex;
            camera.cullingMask = 0;

            _cameraComponent = camera.gameObject.AddComponent<CameraComponent>();
            _cameraComponent.InjectSettings(_settings);

            if (_imageComponent == null)
            {
                RectTransform rect = UIModder.Create(new RectCreationOptions()
                {
                    Name = "Hitbox display",
                    Parent = UIModder.Parents.CanvasHighRes,
                    Position = Vector2.zero,
                    Size = new Vector2(1920, 1080),
                });

                _imageComponent = rect.gameObject.AddComponent<RawImage>();
                _imageComponent.texture = tex;
            }
        }

        ShowHitboxes();
    }

    protected override void OnDeactivate()
    {
        HideHitboxes();
    }

    protected override void OnUpdate()
    {
        if (_cameraComponent != null)
        {
            _cameraComponent.UpdateStatus(IsActive && !IsPauseMenuOpen && !IsInventoryOpen);
        }

        if (IsActive)
        {
            ShowHitboxes();
        }
    }

    private void ShowHitboxes()
    {
        var colliders = Object.FindObjectsOfType<Collider2D>();
        _cameraComponent.UpdateColliders(colliders);
    }

    private void HideHitboxes()
    {
        _cameraComponent.UpdateColliders(null);
    }

    private NewInventoryWidget x_inventoryWidget;
    private bool IsInventoryOpen
    {
        get
        {
            if (x_inventoryWidget == null)
                x_inventoryWidget = Object.FindObjectOfType<NewInventoryWidget>();

            return x_inventoryWidget != null && x_inventoryWidget.currentlyActive;
        }
    }

    private PauseWidget x_pauseWidget;
    private bool IsPauseMenuOpen
    {
        get
        {
            if (x_pauseWidget == null)
                x_pauseWidget = Object.FindObjectOfType<PauseWidget>();

            return x_pauseWidget != null && x_pauseWidget.IsActive();
        }
    }
}
