using Gameplay.UI.Widgets;
using HarmonyLib;

namespace Blasphemous.DebugMod;

// Always allow cursor visibility
[HarmonyPatch(typeof(DebugInformation), "Update")]
class DebugInformation_Update_Patch
{
    public static bool Prefix() => false;
}
