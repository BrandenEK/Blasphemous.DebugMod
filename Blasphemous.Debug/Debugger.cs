using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Debug;

public class Debugger : BlasMod
{
    public Debugger() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    private IModule[] _modules;

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

        _modules =
        [
            new InfoDisplay(),
            new HitboxViewer(hitbox),
            new NoClip(),
            new FreeCam(camera),
        ];
    }

    protected override void OnLateUpdate()
    {
        foreach (var module in _modules)
            module.Update();
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        foreach (var module in _modules)
            module.OnLevelLoaded();
    }

    protected override void OnLevelUnloaded(string oldLevel, string newLevel)
    {
        foreach (var module in _modules)
            module.OnLevelUnloaded();
    }
}
