using Blasphemous.ModdingAPI.Input;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Debug;

public class FreeCam : IModule
{
    private readonly Sprite cameraImage;
    private readonly float _speed;

    private Image cameraObject;
    private Vector3 cameraPosition;

    private bool _enabledFreeCam = false;
    public bool EnabledFreeCam
    {
        get => _enabledFreeCam;
        set
        {
            _enabledFreeCam = value;
            if (value)
            {
                ShowFreeCam();
            }
            else
            {
                HideFreeCam();
            }
        }
    }

    public FreeCam(Sprite camera, float speed)
    {
        cameraImage = camera;
        _speed = speed;
    }

    public void OnLevelLoaded()
    {
    }

    public void OnLevelUnloaded()
    {
        EnabledFreeCam = false;
    }

    public void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown("Free_Cam"))
        {
            EnabledFreeCam = !EnabledFreeCam;
        }

        UpdateFreeCam();
    }

    private void ShowFreeCam()
    {
        if (cameraObject == null)
            CreateCameraImage();
        if (cameraObject != null)
        {
            cameraObject.enabled = true;
        }
    }

    private void HideFreeCam()
    {
        if (cameraObject != null)
        {
            cameraObject.enabled = false;
        }
    }

    private void UpdateFreeCam()
    {
        if (Camera.main == null) return;

        if (EnabledFreeCam)
        {
            float h = Main.Debugger.InputHandler.GetAxis(AxisCode.MoveRHorizontal, true);
            float v = Main.Debugger.InputHandler.GetAxis(AxisCode.MoveRVertical, true);
            var direction = new Vector3(h, v).normalized;

            cameraPosition += direction * _speed * Time.deltaTime;
            Camera.main.transform.position = cameraPosition;
        }
        else
        {
            cameraPosition = Camera.main.transform.position;
        }
    }

    private void CreateCameraImage()
    {
        Transform parent = null;
        foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
        {
            if (obj.name == "PurgePoints")
            {
                parent = obj.transform;
                break;
            }
        }
        if (parent == null) return;

        GameObject camImage = new GameObject("Free Cam Image", typeof(RectTransform));

        RectTransform rect = camImage.GetComponent<RectTransform>();
        rect.gameObject.layer = LayerMask.NameToLayer("UI");
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(0f, -80f);
        rect.sizeDelta = new Vector2(24, 24);

        cameraObject = camImage.AddComponent<Image>();
        cameraObject.sprite = cameraImage;
        cameraObject.enabled = false;
    }
}
