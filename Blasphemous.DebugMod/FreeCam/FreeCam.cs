using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Input;
using Gameplay.UI.Others.UIGameLogic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.FreeCam;

/// <summary>
/// Module for allowing the camera to move anywhere
/// </summary>
internal class FreeCam(Sprite image, float speed) : BaseModule("Free_Cam", true)
{
    private readonly Sprite _image = image;
    private readonly float _speed = speed;

    private Image cameraObject;
    private Vector3 cameraPosition;

    protected override void OnActivate()
    {
        if (cameraObject == null)
            CreateCameraImage();

        cameraObject?.gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        cameraObject?.gameObject.SetActive(false);
    }

    protected override void OnUpdate()
    {
        if (IsActive)
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
        Transform parent = Object.FindObjectsOfType<PlayerPurgePoints>().FirstOrDefault(x => x.name == "PurgePoints")?.transform;
        if (parent == null)
            return;

        cameraObject = UIModder.Create(new RectCreationOptions()
        {
            Name = "FreeCam Icon",
            Parent = parent,
            XRange = Vector2.one,
            YRange = Vector2.one,
            Pivot = Vector2.one,
            Position = new Vector2(0, -80),
            Size = new Vector2(24, 24),
        }).AddImage(new ImageCreationOptions()
        {
            Sprite = _image
        });
    }
}
