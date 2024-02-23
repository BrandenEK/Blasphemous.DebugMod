
namespace Blasphemous.DebugMod;

public class BaseModule(string input)
{
    private readonly string _input = input;

    private bool _active;

    protected bool IsActive
    {
        get => _active;
        set
        {
            _active = value;
            if (value)
                OnActivate();
            else
                OnDeactivate();
        }
    }

    protected virtual void OnLevelLoaded() { }

    protected virtual void OnLevelUnloaded() { }

    protected virtual void OnUpdate() { }

    protected virtual void OnActivate() { }

    protected virtual void OnDeactivate() { }

    /// <summary>
    /// When loading level, perform activation
    /// </summary>
    public void LoadLevel()
    {
        if (IsActive)
            OnActivate();
    }

    /// <summary>
    /// When unloading level, perform deactivation
    /// </summary>
    public void UnloadLevel()
    {
        if (IsActive)
            OnDeactivate();
    }

    /// <summary>
    /// Every frame, check for toggle input and update module if active
    /// </summary>
    public void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown(_input))
        {
            Main.Debugger.Log($"Toggling {_input}");
            IsActive = !IsActive;
        }

        if (IsActive)
        {
            OnUpdate();
        }
    }
}
