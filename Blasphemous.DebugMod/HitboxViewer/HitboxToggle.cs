using Blasphemous.ModdingAPI;
using System.Collections.Generic;

namespace Blasphemous.DebugMod.HitboxViewer;

internal class HitboxToggle
{
    private int _disabledHitboxes = 0;

    internal static readonly List<string> keybindNames =
    [
        "Hitbox_Hazard",
        "Hitbox_Damageable",
        "Hitbox_Player",
        "Hitbox_Sensor",
        "Hitbox_Enemy",
        "Hitbox_Interactable",
        "Hitbox_Trigger",
        "Hitbox_Geometry",
        "Hitbox_Other",
    ];

    private readonly string[] _input = keybindNames.ToArray();

    public bool CheckInput()
    {
        for (int i = 0; i < _input.Length; i++)
        {
            if (Main.Debugger.InputHandler.GetKeyDown(_input[i]))
            {
                _disabledHitboxes ^= (1 << i);
                ModLog.Info($"Toggling type {_input[i]}");
                return true;
            }
        }

        return false;
    }

    public bool ShouldShowHitbox(HitboxType type)
    {
        int hitbox = (int)type;
        return (_disabledHitboxes & (1 << hitbox)) == 0;
    }
}
