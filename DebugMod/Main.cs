using BepInEx;

namespace DebugMod
{
    [BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string MOD_ID = "com.damocles.blasphemous.debug-mod";
        public const string MOD_NAME = "Debug Mod";
        public const string MOD_VERSION = "1.0.2";

        public static Debugger Debugger { get; private set; }

        private void Start()
        {
            Debugger = new Debugger(MOD_ID, MOD_NAME, MOD_VERSION);
        }
    }
}
