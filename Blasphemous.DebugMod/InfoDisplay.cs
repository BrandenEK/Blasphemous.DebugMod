using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod;

/// <summary>
/// Displays debug info on the side
/// </summary>
public class InfoDisplay : IModule
{
    private readonly List<Text> textObjects = new();
    private readonly int _precision;

    internal InfoDisplay(int precision)
    {
        _precision = precision;
    }

    private bool _enabledText = false;
    /// <summary>
    /// Whether this module is active
    /// </summary>
    public bool EnabledText
    {
        get => _enabledText;
        set
        {
            _enabledText = value;
            if (value)
            {
                ShowDebugText();
            }
            else
            {
                HideDebugText();
            }
        }
    }

    /// <summary>
    /// On load, display the text
    /// </summary>
    public void OnLevelLoaded()
    {
        if (EnabledText)
            ShowDebugText();
    }

    /// <summary>
    /// On unload, hide the text
    /// </summary>
    public void OnLevelUnloaded()
    {
        HideDebugText();
    }

    /// <summary>
    /// Every frame, check for input and update the text
    /// </summary>
    public void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown("Info_Display"))
        {
            EnabledText = !EnabledText;
        }

        UpdateDebugText();
    }

    private void ShowDebugText()
    {
        if (textObjects.Count == 0)
        {
            CreateDebugText();
        }

        for (int i = 0; i < textObjects.Count; i++)
        {
            textObjects[i].gameObject.SetActive(true);
        }
    }

    private void HideDebugText()
    {
        for (int i = 0; i < textObjects.Count; i++)
        {
            textObjects[i].gameObject.SetActive(false);
        }
    }

    private void UpdateDebugText()
    {
        if (EnabledText && textObjects.Count >= NUM_TEXT_LINES && Core.Logic.Penitent != null)
        {
            textObjects[0].text = $"Scene: " + Core.LevelManager.currentLevel.LevelName;

            Vector2 position = Core.Logic.Penitent.transform.position;
            textObjects[1].text = $"Position: ({RoundToOne(position.x)}, {RoundToOne(position.y)})";

            Vector2 health = new Vector2(Core.Logic.Penitent.Stats.Life.Current, Core.Logic.Penitent.Stats.Life.CurrentMax);
            textObjects[2].text = $"HP: {RoundToOne(health.x)}/{RoundToOne(health.y)}";

            Vector2 fervour = new Vector2(Core.Logic.Penitent.Stats.Fervour.Current, Core.Logic.Penitent.Stats.Fervour.CurrentMax);
            textObjects[3].text = $"FP: {RoundToOne(fervour.x)}/{RoundToOne(fervour.y)}";
        }

        string RoundToOne(float value)
        {
            return value.ToString($"F{_precision}");
        }
    }

    private void CreateDebugText()
    {
        GameObject textObject = null; Transform parent = null;
        textObjects.Clear();

        foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
        {
            if (obj.name == "PurgePoints") { textObject = obj.transform.GetChild(1).gameObject; break; }
        }
        foreach (PlayerFervour obj in Object.FindObjectsOfType<PlayerFervour>())
        {
            if (obj.name == "Fervour Bar") { parent = obj.transform; break; }
        }
        if (textObject == null || parent == null) return;

        for (int i = 0; i < NUM_TEXT_LINES; i++)
        {
            GameObject newText = Object.Instantiate(textObject, parent);
            newText.name = "DebugText";
            newText.SetActive(false);

            RectTransform rect = newText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(40f, -45 - (i * 18));
            rect.sizeDelta = new Vector2(250f, 18f);

            Text text = newText.GetComponent<Text>();
            text.color = Color.white;
            text.text = string.Empty;
            text.alignment = TextAnchor.MiddleLeft;

            textObjects.Add(text);
        }
    }

    private const int NUM_TEXT_LINES = 4;
}
