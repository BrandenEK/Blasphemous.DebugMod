using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxViewer : BaseModule
{
    private CameraComponent _cameraComponent;
    private RawImage _imageComponent;

    public HitboxSettings Settings { get; }

    public HitboxViewer() : base("Hitbox_Viewer", false)
    {
        Settings = new HitboxSettings()
        {
            Inactive = new ColliderSettings("#4D4E4C", true),
            Hazard = new ColliderSettings("#FF007F", true),
            Damageable = new ColliderSettings("#FFA500", true),
            Player = new ColliderSettings("#00CCCC", true),
            Sensor = new ColliderSettings("#660066", true),
            Enemy = new ColliderSettings("#DD0000", true),
            Interactable = new ColliderSettings("#FFFF33", true),
            Trigger = new ColliderSettings("#0066CC", true),
            Geometry = new ColliderSettings("#00CC00", true),
            Other = new ColliderSettings("#000099", true),
        };

        Main.Instance.gameObject.AddComponent<HitboxWindow>();
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
            _cameraComponent.UpdateStatus(IsActive);
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
}
