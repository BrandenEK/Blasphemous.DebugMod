using BepInEx;

namespace Blasphemous.Debug;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.1.0")]
internal class Main : BaseUnityPlugin
{
    public static Debugger Debugger { get; private set; }

    private void Start()
    {
        Debugger = new Debugger();
    }
}
