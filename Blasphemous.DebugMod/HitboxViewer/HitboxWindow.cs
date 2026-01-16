using UnityEngine;

namespace Blasphemous.DebugMod.HitboxViewer;

public class HitboxWindow : MonoBehaviour
{
    private Rect _window;
    private bool _open = false;
    private HitboxSettings _settings;

    private bool tizona = false;
    private bool cutscenes = false;
    private bool pathnotes = false;
    private bool warp = false;

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
        _window = GUI.Window(0, new Rect(20, ypos, 200, HEIGHT + 20), SettingsWindow, "Hitbox Viewer");
    }

    private void SettingsWindow(int windowID)
    {
        GUI.Label(new Rect(10, 20, 180, 40), "Checkboxes");

        int ypos = 20;
        tizona = GUI.Toggle(new Rect(10, ypos += LINE_HEIGHT, 180, LINE_HEIGHT), tizona, "Tizona fix");
        cutscenes = GUI.Toggle(new Rect(10, ypos += LINE_HEIGHT, 180, LINE_HEIGHT), cutscenes, "Skip cutscenes");
        pathnotes = GUI.Toggle(new Rect(10, ypos += LINE_HEIGHT, 180, LINE_HEIGHT), pathnotes, "Patch notes fix");
        warp = GUI.Toggle(new Rect(10, ypos += LINE_HEIGHT, 180, LINE_HEIGHT), warp, "Always fast travel");
    }

    private const int HEIGHT = 300;
    private const int LINE_HEIGHT = 20;
}
