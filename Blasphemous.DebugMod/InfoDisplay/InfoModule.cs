using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI.Helpers;
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
internal class InfoModule(int precision) : BaseModule("Info_Display", false)
{
    private readonly int _precision = precision;

    private Text infoText;

    protected override void OnActivate()
    {
        if (infoText == null)
            CreateText();

        infoText?.gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        infoText?.gameObject.SetActive(false);
    }

    protected override void OnUpdate()
    {
        if (!IsActive)
            return;

        var sb = new StringBuilder();

        // Scene
        string currentScene = SceneHelper.CurrentScene;
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

        infoText.text = sb.ToString();
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

        infoText = UIModder.Create(new RectCreationOptions()
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
