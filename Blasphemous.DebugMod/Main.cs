using BepInEx;

namespace Blasphemous.DebugMod;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "3.0.0")]
[BepInDependency("Blasphemous.Framework.UI", "0.2.0")]
internal class Main : BaseUnityPlugin
{
    public static Debugger Debugger { get; private set; }
    public static Main Instance { get; private set; }

    private void Start()
    {
        Debugger = new Debugger();
        Instance = this;
    }
}
