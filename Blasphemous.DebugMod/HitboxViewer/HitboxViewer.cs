using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

/// <summary>
/// Module for displaying hitbox info on colliders
/// </summary>
internal class HitboxViewer(Sprite image, float delay) : BaseModule("Hitbox_Viewer", false)
{
    private readonly Dictionary<int, HitboxData> _activeHitboxes = new();
    private readonly Sprite _image = image;
    private readonly float _totalDelay = delay;

    private float _currentDelay = 0f;

    protected override void OnActivate()
    {
        // Foreach collider in the scene, add a HitboxData if it doesnt already have one
        var foundColliders = new List<int>();
        foreach (Collider2D collider in Object.FindObjectsOfType<Collider2D>())
        {
            int id = collider.gameObject.GetInstanceID();
            foundColliders.Add(id);

            if (!_activeHitboxes.ContainsKey(id))
            {
                _activeHitboxes.Add(id, new HitboxData(collider, _image));
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
    }

    private const string GEOMETRY_NAME = "GEO_Block";
}
