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
        if (!Main.Debugger.HitboxModule.IsActive)
        {
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

        bool inactive = ReadCheckbox(ypos += LINE_HEIGHT, "Inactive", _settings.Inactive.Visible);
        bool Hazard = ReadCheckbox(ypos += LINE_HEIGHT, "Hazard", _settings.Hazard.Visible);
        bool Damageable = ReadCheckbox(ypos += LINE_HEIGHT, "Damageable", _settings.Damageable.Visible);
        bool Player = ReadCheckbox(ypos += LINE_HEIGHT, "Player", _settings.Player.Visible);
        bool Sensor = ReadCheckbox(ypos += LINE_HEIGHT, "Sensor", _settings.Sensor.Visible);
        bool Enemy = ReadCheckbox(ypos += LINE_HEIGHT, "Enemy", _settings.Enemy.Visible);
        bool Interactable = ReadCheckbox(ypos += LINE_HEIGHT, "Interactable", _settings.Interactable.Visible);
        bool Geometry = ReadCheckbox(ypos += LINE_HEIGHT, "Geometry", _settings.Geometry.Visible);
        bool Trigger = ReadCheckbox(ypos += LINE_HEIGHT, "Trigger", _settings.Trigger.Visible);
        bool Other = ReadCheckbox(ypos += LINE_HEIGHT, "Other", _settings.Other.Visible);
    }

    private bool ReadCheckbox(int ypos, string label, bool value)
    {
        return GUI.Toggle(new Rect(10, ypos, WIDTH - 20, LINE_HEIGHT), value, label);
    }

    private const int WIDTH = 200;
    private const int HEIGHT = 300;
    private const int LINE_HEIGHT = 20;
}
