
namespace Blasphemous.DebugMod;

internal interface IModule
{
    public void OnLevelLoaded();

    public void OnLevelUnloaded();

    public void Update();
}
