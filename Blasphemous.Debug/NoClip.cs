using Blasphemous.ModdingAPI.Input;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Blasphemous.Debug;

public class NoClip : IModule
{
    private readonly float _speed;

    private Vector3 playerPosition;

    private bool _enabledFreeMove = false;
    public bool EnabledFreeMove
    {
        get => _enabledFreeMove;
        set
        {
            if (_enabledFreeMove && !value)
            {
                PlayerController.enabled = true;
                PlayerFloorCollider.enabled = true;
                PlayerDamageArea.enabled = true;
                PlayerTrapArea.enabled = true;
            }
            _enabledFreeMove = value;
        }
    }

    public NoClip(float speed)
    {
        _speed = speed;
    }

    public void OnLevelLoaded()
    {
    }

    public void OnLevelUnloaded()
    {
        EnabledFreeMove = false;
    }

    public void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown("No_Clip"))
        {
            EnabledFreeMove = !EnabledFreeMove;
        }

        UpdateFreeMove();
    }

    private void UpdateFreeMove()
    {
        if (Core.Logic.Penitent == null) return;

        if (EnabledFreeMove)
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
