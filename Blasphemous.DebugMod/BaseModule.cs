using Blasphemous.ModdingAPI;

namespace Blasphemous.DebugMod;

/// <summary>
/// Handles activation/deactivation on load and toggle in update
/// </summary>
public class BaseModule(string input, bool autoDeactivate)
{
    private protected readonly string _input = input;
    private readonly bool _autoDeactivate = autoDeactivate;

    private bool _active;

    /// <summary>
    /// Whether this module is active
    /// </summary>
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

    /// <summary>
    /// Called when toggled on or when loading a new scene while active
    /// </summary>
    protected virtual void OnActivate() { }

    /// <summary>
    /// Called when toggled off or when unloading a scene while active
    /// </summary>
    protected virtual void OnDeactivate() { }

    /// <summary>
    /// Called every frame a game scene is loaded
    /// </summary>
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
    public virtual void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown(_input))
        {
            ModLog.Info($"Toggling {_input}");
            IsActive = !IsActive;
        }

        OnUpdate();
    }
}
