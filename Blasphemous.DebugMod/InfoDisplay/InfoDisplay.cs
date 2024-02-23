﻿using Blasphemous.Framework.UI;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.InfoDisplay;

/// <summary>
/// Module for displaying debug information
/// </summary>
internal class InfoDisplay(int precision) : BaseModule("Info_Display")
{
    private readonly int _precision = precision;

    private Text _infoText;

    protected override void OnUpdate()
    {
        var sb = new StringBuilder();

        // Scene
        string currentScene = Main.Debugger.LoadStatus.CurrentScene;
        sb.AppendLine($"Scene: {currentScene}");

        // Position
        Vector2 playerPosition = Core.Logic.Penitent.transform.position;
        sb.AppendLine($"Position: {RoundDecimal(playerPosition.x)}, {RoundDecimal(playerPosition.y)}");

        // Health
        float currentHealth = Core.Logic.Penitent.Stats.Life.Current;
        float maxHealth = Core.Logic.Penitent.Stats.Life.CurrentMax;
        sb.AppendLine($"Health: {RoundDecimal(currentHealth)}/{RoundDecimal(maxHealth)}");

        // Fervour
        float currentFervour = Core.Logic.Penitent.Stats.Fervour.Current;
        float maxFervour = Core.Logic.Penitent.Stats.Fervour.CurrentMax;
        sb.AppendLine($"Fervour: {RoundDecimal(currentFervour)}/{RoundDecimal(maxFervour)}");

        _infoText.text = sb.ToString();
    }

    protected override void OnActivate()
    {
        if (_infoText == null)
            CreateText();

        _infoText?.gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        _infoText?.gameObject.SetActive(false);
    }

    private string RoundDecimal(float num)
    {
        int precision = System.Math.Max(_precision, 1);
        return num.ToString("F" + precision);
    }

    private void CreateText()
    {
        Transform parent = Object.FindObjectsOfType<PlayerFervour>().FirstOrDefault(x => x.name == "Fervour Bar")?.transform;
        if (parent == null)
            return;

        _infoText = UIModder.Create(new RectCreationOptions()
        {
            Name = "Debug Text",
            Parent = parent,
            XRange = Vector2.zero,
            YRange = Vector2.one,
            Pivot = new Vector2(0, 1),
            Position = new Vector2(40, -45),
            Size = new Vector2(250, 250)
        }).AddText(new TextCreationOptions()
        {
            Alignment = TextAnchor.UpperLeft
        });
    }
}
