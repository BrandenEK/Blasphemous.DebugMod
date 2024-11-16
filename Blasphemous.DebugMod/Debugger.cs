using Blasphemous.DebugMod.FreeCam;
using Blasphemous.DebugMod.HitboxViewer;
using Blasphemous.DebugMod.InfoDisplay;
using Blasphemous.DebugMod.NoClip;
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
            { PenitentInfoDisplayModule.keybindName, KeyCode.F1 },
            { HitboxViewerModule.keybindName, KeyCode.F2 },
            { NoClipModule.keybindName, KeyCode.F3 },
            { FreeCamModule.keybindName, KeyCode.F4 },
            { HitboxToggle.keybindNames[0], KeyCode.Keypad1 },
            { HitboxToggle.keybindNames[1], KeyCode.Keypad2 },
            { HitboxToggle.keybindNames[2], KeyCode.Keypad3 },
            { HitboxToggle.keybindNames[3], KeyCode.Keypad4 },
            { HitboxToggle.keybindNames[4], KeyCode.Keypad5 },
            { HitboxToggle.keybindNames[5], KeyCode.Keypad6 },
            { HitboxToggle.keybindNames[6], KeyCode.Keypad7 },
            { HitboxToggle.keybindNames[7], KeyCode.Keypad8 },
            { HitboxToggle.keybindNames[8], KeyCode.Keypad9 },
        });

        FileHandler.LoadDataAsSprite("camera.png", out Sprite camera, new SpriteImportOptions()
        {
            PixelsPerUnit = 24
        });

        Config cfg = ConfigHandler.Load<Config>();
        ConfigHandler.Save(cfg);

        _modules =
        [
            new InfoDisplay.PenitentInfoDisplayModule(cfg.infoPrecision),
            new HitboxViewer.HitboxViewerModule(cfg.hitboxUpdateDelay),
            new NoClip.NoClipModule(cfg.playerSpeed),
            new FreeCam.FreeCamModule(camera, cfg.cameraSpeed),
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
