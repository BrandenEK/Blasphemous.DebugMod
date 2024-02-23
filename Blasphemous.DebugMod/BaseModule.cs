
namespace Blasphemous.DebugMod;

public class BaseModule(string input, bool autoDeactivate)
{
    private readonly string _input = input;
    private readonly bool _autoDeactivate = autoDeactivate;

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

    //protected virtual void OnLevelLoaded() { }

    //protected virtual void OnLevelUnloaded() { }

    protected virtual void OnActivate() { }

    protected virtual void OnDeactivate() { }

    protected virtual void OnUpdate() { }

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

        if (_autoDeactivate)
            _active = false;
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

        OnUpdate();
    }
}
