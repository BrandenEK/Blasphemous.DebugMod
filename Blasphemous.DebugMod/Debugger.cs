﻿using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
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
        });

        FileHandler.LoadDataAsSprite("hitbox.png", out Sprite hitbox, new SpriteImportOptions()
        {
            PixelsPerUnit = 1
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
            new HitboxViewer.HitboxViewer(hitbox),
            new NoClip.NoClip(cfg.playerSpeed),
            new FreeCam.FreeCam(camera, cfg.cameraSpeed),
        ];
    }

    /// <summary>
    /// Update modules
    /// </summary>
    protected override void OnLateUpdate()
    {
        if (!LoadStatus.GameSceneLoaded)
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
