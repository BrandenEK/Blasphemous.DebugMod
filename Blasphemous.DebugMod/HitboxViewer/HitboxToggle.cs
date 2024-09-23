using Blasphemous.ModdingAPI;

namespace Blasphemous.DebugMod.HitboxViewer;

internal class HitboxToggle
{
    private int _disabledHitboxes = 0;

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

    private readonly string[] _input =
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
}
