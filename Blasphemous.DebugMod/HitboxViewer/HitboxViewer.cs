using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxViewer() : BaseModule("Hitbox_Viewer", false)
{
    private readonly HitboxToggle _toggle = new();

    private CameraComponent _cameraComponent;
    private Camera _camera;

    protected override void OnActivate()
    {
        //Object.FindObjectsOfType<Camera>().First(x => x.name == "scene composition camera");
        //Object.FindObjectsOfType<Camera>().First(x => x.name == "Virtual Camera");

        foreach (var cam in Object.FindObjectsOfType<Camera>())
        {
            ModLog.Error(cam.gameObject.name);
            ModLog.Info(cam.enabled);
            ModLog.Info(cam.aspect);
            ModLog.Info(cam.targetDisplay);
            ModLog.Info(cam.depth);
            ModLog.Info(cam.clearFlags);
            ModLog.Info(cam.targetTexture?.name);
            ModLog.Info(cam.targetTexture?.width);
            ModLog.Info(cam.targetTexture?.height);

            if (cam.name == "Camera")
            {
                //cam.targetTexture = new RenderTexture(1920, 1080, 24);
                //cam.targetTexture.Create();
            }
        }

        foreach (var texHolder in Object.FindObjectsOfType<CameraTextureHolder>())
        {
            //ModLog.Error(texHolder.gameObject.name);
            //if (texHolder.name != "Camera")
            //    continue;

            //texHolder._renderTexture = new RenderTexture(1920, 1080, 24);
            //texHolder._renderTexture.Create();
        }

        var camera = Camera.main;

        if (_camera == null)
        {
            ModLog.Info("Creating new hitbox camera");
            var tex = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            tex.Create();

            var camObject = new GameObject("Hitbox Camera");
            camObject.transform.SetParent(camera.transform, false);

            _camera = camObject.AddComponent<Camera>();
            _camera.orthographic = camera.orthographic;
            _camera.orthographicSize = camera.orthographicSize;
            _camera.aspect = camera.aspect;
            _camera.depth = 10;
            _camera.clearFlags = CameraClearFlags.Color;
            _camera.backgroundColor = new Color32(0, 0, 0, 0);
            _camera.targetTexture = tex;
            _camera.cullingMask = 0;

            _cameraComponent = _camera.gameObject.AddComponent<CameraComponent>();

            foreach (Component c in _camera.GetComponents<Component>())
                ModLog.Warn(c.GetType().Name);

            RectTransform rect = UIModder.Create(new RectCreationOptions()
            {
                Name = "Hitbox texture",
                Parent = UIModder.Parents.CanvasHighRes,
                Position = Vector2.zero,
                Size = new Vector2(1920, 1080),
            });


            image = rect.gameObject.AddComponent<RawImage>();
            image.texture = tex;
        }

        ShowHitboxes();
    }

    public static RawImage image;

    protected override void OnDeactivate()
    {
        HideHitboxes();
    }

    protected override void OnUpdate()
    {
        if (_cameraComponent != null)
        {
            _camera.enabled = true;
            _cameraComponent.UpdateStatus(IsActive);
        }

        if (IsActive)
        {
            _toggle.CheckInput();
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
