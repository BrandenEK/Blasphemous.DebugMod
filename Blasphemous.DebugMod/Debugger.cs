using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using Blasphemous.ModdingAPI.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DebugMod;

/// <summary>
/// Handles different modules for debugging purposes
/// </summary>
public class Debugger : BlasMod
{
    internal Debugger() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private BaseModule[] _modules;

    /// <summary>
    /// Register handlers and load images
    /// </summary>
    protected override void OnInitialize()
    {
        InputHandler.RegisterDefaultKeybindings(new Dictionary<string, KeyCode>()
        {
            { "Info_Display", KeyCode.F1 },
            { "Hitbox_Viewer", KeyCode.F2 },
            { "No_Clip", KeyCode.F3 },
            { "Free_Cam", KeyCode.F4 },
            { "Hitbox_Hazard", KeyCode.Keypad1 },
            { "Hitbox_Damageable", KeyCode.Keypad2 },
            { "Hitbox_Player", KeyCode.Keypad3 },
            { "Hitbox_Sensor", KeyCode.Keypad4 },
            { "Hitbox_Enemy", KeyCode.Keypad5 },
            { "Hitbox_Interactable", KeyCode.Keypad6 },
            { "Hitbox_Trigger", KeyCode.Keypad7 },
            { "Hitbox_Geometry", KeyCode.Keypad8 },
            { "Hitbox_Other", KeyCode.Keypad9 },
        });

        FileHandler.LoadDataAsSprite("camera.png", out Sprite camera, new SpriteImportOptions()
        {
            PixelsPerUnit = 24
        });

        Config cfg = ConfigHandler.Load<Config>();
        ConfigHandler.Save(cfg);

        _modules =
        [
            new InfoDisplay.InfoDisplay(cfg.infoPrecision),
            new HitboxViewer.HitboxViewer(cfg.hitboxUpdateDelay),
            new NoClip.NoClip(cfg.playerSpeed),
            new FreeCam.FreeCam(camera, cfg.cameraSpeed),
        ];
    }

    /// <summary>
    /// Update modules
    /// </summary>
    protected override void OnLateUpdate()
    {
        if (!SceneHelper.GameSceneLoaded)
            return;

        foreach (var module in _modules)
            module.Update();
    }

    /// <summary>
    /// Load modules
    /// </summary>
    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        if (newLevel == "MainMenu")
            return;

        foreach (var module in _modules)
            module.LoadLevel();
    }

    /// <summary>
    /// Unload modules
    /// </summary>
    protected override void OnLevelUnloaded(string oldLevel, string newLevel)
    {
        if (newLevel == "MainMenu")
            return;

        foreach (var module in _modules)
            module.UnloadLevel();
    }
}
