using Blasphemous.DebugMod.FreeCam;
using Blasphemous.DebugMod.NoClip;
using Blasphemous.ModdingAPI;
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

    internal CameraModule CameraModule { get; private set; }
    internal FlyModule FlyModule { get; private set; }
    internal HitboxViewer.HitboxViewer HitboxModule { get; private set; }

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

        Config cfg = ConfigHandler.Load<Config>();
        ConfigHandler.Save(cfg);

        CameraModule = new CameraModule(cfg.cameraSpeed);
        FlyModule = new FlyModule(cfg.playerSpeed);
        HitboxModule = new HitboxViewer.HitboxViewer();

        _modules =
        [
            new InfoDisplay.InfoDisplay(cfg.infoPrecision),
            HitboxModule,
            FlyModule,
            CameraModule,
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
