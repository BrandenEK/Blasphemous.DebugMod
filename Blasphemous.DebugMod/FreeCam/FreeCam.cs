using Blasphemous.ModdingAPI.Input;
using Framework.Managers;
using UnityEngine;

namespace Blasphemous.DebugMod.FreeCam;

/// <summary>
/// Module for allowing the camera to move anywhere
/// </summary>
internal class FreeCam(float speed) : BaseModule("Free_Cam", true)
{
    private readonly float _speed = speed;

    private Vector3 cameraPosition;

    protected override void OnUpdate()
    {
        if (IsActive && !Core.Input.HasBlocker("CONSOLE"))
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
}
