using Blasphemous.ModdingAPI.Input;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.DebugMod.NoClip;

/// <summary>
/// Module for disabling collision and gravity
/// </summary>
internal class NoClipModule(float speed) : BaseModule(keybindName, true)
{
    private readonly float _speed = speed;
    internal static readonly string keybindName = "No_Clip";

    private Vector3 playerPosition;

    protected override void OnDeactivate()
    {
        PlayerController.enabled = true;
        PlayerFloorCollider.enabled = true;
        PlayerDamageArea.enabled = true;
        PlayerTrapArea.enabled = true;
    }

    protected override void OnUpdate()
    {
        if (IsActive)
        {
            PlayerController.enabled = false;
            PlayerFloorCollider.enabled = false;
            PlayerDamageArea.enabled = false;
            PlayerTrapArea.enabled = false;

            float h = Main.Debugger.InputHandler.GetAxis(AxisCode.MoveHorizontal, true);
            float v = Main.Debugger.InputHandler.GetAxis(AxisCode.MoveVertical, true);
            var direction = new Vector3(h, v).normalized;

            playerPosition += direction * _speed * Time.deltaTime;
            Core.Logic.Penitent.transform.position = playerPosition;
        }
        else
        {
            playerPosition = Core.Logic.Penitent.transform.position;
        }
    }

    private PlatformCharacterController _playerController;
    private PlatformCharacterController PlayerController
    {
        get
        {
            if (_playerController == null)
                _playerController = Core.Logic.Penitent.PlatformCharacterController;
            return _playerController;
        }
    }
    private SmartPlatformCollider _playerFloorCollider;
    private SmartPlatformCollider PlayerFloorCollider
    {
        get
        {
            if (_playerFloorCollider == null)
                _playerFloorCollider = Core.Logic.Penitent.GetComponent<SmartPlatformCollider>();
            return _playerFloorCollider;
        }
    }
    private BoxCollider2D _playerDamageArea;
    private BoxCollider2D PlayerDamageArea
    {
        get
        {
            if (_playerDamageArea == null)
                _playerDamageArea = Core.Logic.Penitent.GetComponentInChildren<BoxCollider2D>();
            return _playerDamageArea;
        }
    }
    private BoxCollider2D _playerTrapArea;
    private BoxCollider2D PlayerTrapArea
    {
        get
        {
            if (_playerTrapArea == null)
                _playerTrapArea = Core.Logic.Penitent.GetComponentInChildren<CheckTrap>().GetComponent<BoxCollider2D>();
            return _playerTrapArea;
        }
    }
}
