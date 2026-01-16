using System.Linq;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class CameraComponent : MonoBehaviour
{
    private Camera _camera;
    private HitboxSettings _settings;

    private Material _material;
    private Bounds _camBounds;

    private Collider2D[] _cachedColliders = null;
    private bool _isShowing = false;

    private int _segments = 32;
    private float _angleStep = 2 * Mathf.PI / 32;

    public void InjectSettings(HitboxSettings settings)
    {
        _settings = settings;
    }

    public void UpdateColliders(Collider2D[] colliders)
    {
        _cachedColliders = colliders;
    }

    public void UpdateStatus(bool isShowing)
    {
        _isShowing = isShowing;
    }

    void Awake()
    {
        CacheLineMaterial();
        _camera = GetComponent<Camera>();
    }

    private void CacheLineMaterial()
    {
        // Unity has a built-in shader that is useful for drawing simple colored things
        var shader = Shader.Find("Hidden/Internal-Colored");
        _material = new Material(shader);
        _material.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        _material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        _material.SetInt("_ZWrite", 0);
    }

    private void CacheCameraBounds()
    {
        float height = _camera.orthographicSize;
        float width = _camera.aspect * height;
        _camBounds = new(_camera.transform.position, new Vector3(width, height) * 2);
    }

    private void OnPostRender()
    {
        if (!_isShowing || _cachedColliders == null)
            return;

        // Activate render texture
        RenderTexture activeTexture = RenderTexture.active;
        RenderTexture.active = _camera.targetTexture;

        _material.SetPass(0);
        CacheCameraBounds();

        GL.LoadOrtho();
        GL.Begin(GL.LINES);

        foreach (var info in _cachedColliders.Select(CalculateInfo).OrderBy(x => x.Type))
        {
            if (!info.IsVisible)
                continue;

            GL.Color(_settings[info.Type].Color);

            switch (info.Collider.GetType().Name)
            {
                case "BoxCollider2D":
                    RenderBox(info.Collider as BoxCollider2D);
                    break;
                case "CircleCollider2D":
                    RenderCircle(info.Collider as CircleCollider2D);
                    break;
                case "CapsuleCollider2D":
                    RenderCapsule(info.Collider as CapsuleCollider2D);
                    break;
                case "PolygonCollider2D":
                    RenderPolygon(info.Collider as PolygonCollider2D);
                    break;
                default:
                    break;
            }
        }

        GL.End();

        // Deactivate render texture
        RenderTexture.active = activeTexture;
    }

    void RenderBox(BoxCollider2D collider)
    {
        Vector2 halfSize = collider.size / 2f;
        var topLeft = CalculateViewport(collider, new Vector2(-halfSize.x, halfSize.y));
        var topRight = CalculateViewport(collider, new Vector2(halfSize.x, halfSize.y));
        var bottomRight = CalculateViewport(collider, new Vector2(halfSize.x, -halfSize.y));
        var bottomLeft = CalculateViewport(collider, new Vector2(-halfSize.x, -halfSize.y));

        GL.Vertex(topLeft);
        GL.Vertex(topRight);

        GL.Vertex(topRight);
        GL.Vertex(bottomRight);

        GL.Vertex(bottomRight);
        GL.Vertex(bottomLeft);

        GL.Vertex(bottomLeft);
        GL.Vertex(topLeft);
    }

    void RenderCircle(CircleCollider2D collider)
    {
        float radius = collider.radius;

        Vector3 start = CalculateViewport(collider, new Vector2(radius, 0));
        GL.Vertex(start);

        for (int i = 1; i < _segments; i++)
        {
            float angle = i * _angleStep;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            Vector2 point = CalculateViewport(collider, new Vector2(x, y));
            GL.Vertex(point);
            GL.Vertex(point);
        }

        GL.Vertex(start);
    }

    void RenderCapsule(CapsuleCollider2D collider)
    {
        float radius = collider.size.x / 2;
        float height = collider.size.y / 2;

        Vector3 start = CalculateViewport(collider, new Vector2(0, height));
        GL.Vertex(start);

        for (int i = 1; i < _segments; i++)
        {
            float angle = i * _angleStep;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * height;

            Vector2 point = CalculateViewport(collider, new Vector2(x, y));
            GL.Vertex(point);
            GL.Vertex(point);
        }

        GL.Vertex(start);
    }

    void RenderPolygon(PolygonCollider2D collider)
    {
        if (collider.pathCount == 0)
            return;

        Vector2[] points = collider.GetPath(0);

        if (points.Length < 3)
            return;

        Vector3 start = CalculateViewport(collider, points[0]);
        GL.Vertex(start);

        for (int i = 1; i < points.Length; i++)
        {
            Vector3 point = CalculateViewport(collider, points[i]);

            GL.Vertex(point);
            GL.Vertex(point);
        }

        GL.Vertex(start);
    }

    private Vector2 CalculateViewport(Collider2D collider, Vector3 point)
    {
        Transform t = collider.transform;
        Vector2 offset = collider.offset;
        Vector3 position = t.position;
        Vector3 scale = t.lossyScale;

        // Apply offset
        point.x += offset.x;
        point.y += offset.y;

        // Apply rotation
        point = t.rotation * point;

        // Apply scale
        point.x *= scale.x;
        point.y *= scale.y;

        // Convert to world
        point.x += position.x;
        point.y += position.y;

        return _camera.WorldToViewportPoint(point);
    }

    private HitboxInfo CalculateInfo(Collider2D collider)
    {
        // Verify collider still exists
        if (collider == null)
            return new HitboxInfo(collider, HitboxType.Invalid, false);

        // Verify collider is in camera bounds
        if (collider.bounds.min.x > _camBounds.max.x || collider.bounds.min.y > _camBounds.max.y ||
            collider.bounds.max.x < _camBounds.min.x || collider.bounds.max.y < _camBounds.min.y)
            return new HitboxInfo(collider, HitboxType.Invalid, false);

        // Verify collider is a valid size
        Vector2 size = collider.bounds.extents * 2;
        if (collider.enabled && (size.x < MIN_SIZE || size.x > MAX_SIZE || size.y < MIN_SIZE || size.y > MAX_SIZE))
            return new HitboxInfo(collider, HitboxType.Invalid, false);

        HitboxType type = collider.GetHitboxType();

        // Verify collider is toggled on
        if (!_settings[type].Visible)
            return new HitboxInfo(collider, type, false);

        return new HitboxInfo(collider, type, true);
    }

    private const float MIN_SIZE = 0.1f;
    private const float MAX_SIZE = 100f;
}
