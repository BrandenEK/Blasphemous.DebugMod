using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class HitboxWindow : MonoBehaviour
{
    private Rect _window;
    private bool _open = false;
    private HitboxSettings _settings;

    public void InjectSettings(HitboxSettings settings)
    {
        _settings = settings;
    }

    private void OnGUI()
    {
        if (!Main.Debugger.HitboxModule.IsActive || !SceneHelper.GameSceneLoaded)
        {
            Cursor.visible = false;
            _open = false;
            return;
        }

        Cursor.visible = true;
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
            _open = _window.Contains(e.mousePosition);

        int ypos = Screen.height - (_open ? HEIGHT : 17);
        _window = GUI.Window(0, new Rect(20, ypos, WIDTH, HEIGHT + 20), SettingsWindow, "Hitbox Viewer");
    }

    private void SettingsWindow(int windowID)
    {
        int ypos = 20;

        foreach (var setting in _settings)
        {
            bool value = ReadCheckbox(ypos += LINE_HEIGHT, setting.Type.ToString(), setting.Visible);

            if (value != setting.Visible)
            {
                setting.Visible = value;
                ModLog.Info($"Toggling hitbox type {setting.Type} to {value}");
            }
        }
    }

    private bool ReadCheckbox(int ypos, string label, bool value)
    {
        return GUI.Toggle(new Rect(10, ypos, WIDTH - 20, LINE_HEIGHT), value, label);
    }

    private const int WIDTH = 200;
    private const int HEIGHT = 300;
    private const int LINE_HEIGHT = 20;
}
