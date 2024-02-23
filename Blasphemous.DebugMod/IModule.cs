
namespace Blasphemous.DebugMod;

internal interface IModule
{
    //public bool IsActive { get; set; }

    public void OnLevelLoaded();

    public void OnLevelUnloaded();

    public void Update();
}
