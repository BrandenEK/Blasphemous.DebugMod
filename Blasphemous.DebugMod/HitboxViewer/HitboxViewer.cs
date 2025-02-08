using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxViewer(float delay) : BaseModule("Hitbox_Viewer", false)
{
    private readonly HitboxToggle _toggle = new();
    private readonly Dictionary<int, HitboxData> _activeHitboxes = new();
    private readonly float _totalDelay = delay;

    private float _currentDelay = 0f;

    private CameraComponent _cameraComponent;

    protected override void OnActivate()
    {
        var camera = Camera.main;//Object.FindObjectsOfType<Camera>().First(x => x.name == "Virtual Camera");
        if (camera.GetComponent<CameraComponent>() == null)
        {
            _cameraComponent = camera.gameObject.AddComponent<CameraComponent>();
        }

        ShowHitboxes();
    }

    protected override void OnDeactivate()
    {
        HideHitboxes();
    }

    protected override void OnUpdate()
    {
        _cameraComponent.UpdateStatus(IsActive);

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
