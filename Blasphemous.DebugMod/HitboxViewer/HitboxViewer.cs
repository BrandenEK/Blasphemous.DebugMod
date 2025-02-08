using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;
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
        //foreach (var cam in Object.FindObjectsOfType<Camera>())
        //{
        //    ModLog.Info(cam.name);
        //    ModLog.Warn(cam.orthographicSize);
        //}
        var camera = Camera.main;//Object.FindObjectsOfType<Camera>().First(x => x.name == "Virtual Camera");
        if (camera.GetComponent<CameraComponent>() == null)
        {
            _cameraComponent = camera.gameObject.AddComponent<CameraComponent>();
        }

        // Foreach collider in the scene, add a HitboxData if it doesnt already have one
        var foundColliders = new List<int>();
        foreach (Collider2D collider in Object.FindObjectsOfType<Collider2D>())
        {
            int id = collider.gameObject.GetInstanceID();
            foundColliders.Add(id);

            if (!_activeHitboxes.ContainsKey(id))
            {
                _activeHitboxes.Add(id, new HitboxData(collider, _toggle));
            }
        }

        // Foreach collider in the list that wasn't found, remove it
        var destroyedColliders = new List<int>();
        foreach (int colliderId in _activeHitboxes.Keys)
        {
            if (!foundColliders.Contains(colliderId))
                destroyedColliders.Add(colliderId);
        }
        foreach (int colliderId in destroyedColliders)
        {
            _activeHitboxes[colliderId].DestroyHitbox();
            _activeHitboxes.Remove(colliderId);
        }

        // Reset timer
        _currentDelay = 0;
    }

    protected override void OnDeactivate()
    {
        foreach (HitboxData hitbox in _activeHitboxes.Values)
        {
            hitbox.DestroyHitbox();
        }

        _activeHitboxes.Clear();
    }

    protected override void OnUpdate()
    {
        if (!IsActive)
            return;

        _currentDelay += Time.deltaTime;
        if (_currentDelay >= _totalDelay)
        {
            OnActivate();
        }

        if (_toggle.CheckInput())
        {
            OnDeactivate();
            OnActivate();
        }
    }
}
