
namespace Blasphemous.Debug;

public interface IModule
{
    public void OnLevelLoaded();

    public void OnLevelUnloaded();

    public void Update();
}
