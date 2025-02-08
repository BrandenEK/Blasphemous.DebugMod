using System.Linq;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class CameraComponent : MonoBehaviour
{
    private Camera _camera;

    private Material _material;
    private Bounds _camBounds;

    private Collider2D[] _cachedColliders = null;
    private bool _isShowing = false;

    private int _segments = 32;
    private float _angleStep = 2 * Mathf.PI / 32;

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
        _camera = /*Object.FindObjectsOfType<Camera>().First(x => x.name == "scene composition camera");*/ GetComponent<Camera>();
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
        _material.SetPass(0);
        CacheCameraBounds();

        GL.LoadOrtho();
        GL.Begin(GL.LINES);

        // test
        GL.Color(Color.red);
        GL.Vertex(new Vector3(0, 0, 0));
        GL.Vertex(new Vector3(1, 1, 0));
        // test

        GL.End();
    }
}
