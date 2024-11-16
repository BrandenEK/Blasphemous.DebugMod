using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.FrameworkCore.Attributes;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DebugMod.InfoDisplay;

/// <summary>
/// Module for displaying penitent information
/// </summary>
internal class PenitentInfoDisplay(int precision) : BaseModule("Info_Display", false)
{
    private readonly int _precision = precision;
    internal static readonly string gameObjectName = "Penitent info display";

    private Text infoText;
    private DisplayType currentDisplayType;

    private enum DisplayType
    {
        HealthFervourFlask,
        Defenses,
        Attacks,
        Positional
    }

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

    public override void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown(_input))
        {
            ModLog.Info($"Toggling {_input}");
            if (IsActive == false)
            {
                IsActive = true;
                currentDisplayType = DisplayType.HealthFervourFlask;
                ModLog.Info($"Turning on {gameObjectName}");
                ModLog.Info($"Displaying {currentDisplayType}");
            }
            else
            {
                // switch to the next display option
                if (currentDisplayType == DisplayType.Positional)
                {
                    // when reaching the last display option, deactivate display at toggle
                    IsActive = false;
                    ModLog.Info($"Turning off {gameObjectName}");
                }
                else
                {
                    currentDisplayType++;
                    ModLog.Info($"Displaying {currentDisplayType}");
                }
            }
        }

        OnUpdate();
    }

    protected override void OnUpdate()
    {
        if (!IsActive)
            return;

        UpdateText(currentDisplayType);
    }

    private string RoundFixedPoint(float num)
    {
        int precision = System.Math.Max(_precision, 1);
        return num.ToString("F" + precision);
    }

    private string RoundPercentage(float num)
    {
        int precision = System.Math.Max(_precision, 1);
        return num.ToString("P" + precision);
    }

    private void CreateText()
    {
        Transform parent = Object.FindObjectsOfType<PlayerFervour>().FirstOrDefault(x => x.name == "Fervour Bar")?.transform;
        if (parent == null)
            return;

        Vector2 rectSize = new Vector2(250, 250);

        infoText = UIModder.Create(new RectCreationOptions()
        {
            Name = gameObjectName,
            Parent = parent,
            XRange = Vector2.zero,
            YRange = Vector2.one,
            Pivot = new Vector2(0, 1),
            Position = new Vector2(40, -45),
            Size = rectSize,
        }).AddText(new TextCreationOptions()
        {
            Alignment = TextAnchor.UpperLeft,
            FontSize = 14,
            Font = UIModder.Fonts.Arial,
            WordWrap = true,
            Color = Color.white,
        });

        // get UI gameObject and attach a sprite gameObject to it
        GameObject obj = infoText.gameObject;
        GameObject spriteObject = new($"{gameObjectName}_sprite");
        spriteObject.transform.SetParent(obj.transform, false);

        // create a 1*1 sprite for coloring use
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);

        // add sprite to the UI gameObject
        spriteObject.AddComponent<SpriteRenderer>();    
        SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 10000;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2 (0, 1));
        sr.color = new Color(0, 0, 0, 0.7f);
        spriteObject.transform.localScale = new Vector3(rectSize.x, rectSize.y, 1);
    }

    private void UpdateText(DisplayType displayType)
    {
        var sb = new StringBuilder();
        var penitent = Core.Logic.Penitent;

        float current;
        float max;
        float baseValue;
        int upgradeCount;
        float upgradeValue;
        float bonus;

        switch (displayType)
        {
            case DisplayType.HealthFervourFlask:
                // Health
                var penitentHealth = penitent.Stats.Life;
                current = penitentHealth.Current;
                max = penitentHealth.CurrentMax;
                baseValue = penitentHealth.Base;
                upgradeCount = penitentHealth.GetUpgrades();
                upgradeValue = Traverse.Create(penitentHealth).Field("_upgradeValue").GetValue<float>();
                bonus = max - baseValue - upgradeCount * upgradeValue;
                sb.AppendLine($"Health: {RoundFixedPoint(current)}/{RoundFixedPoint(max)}");
                sb.AppendLine($"Max Health: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeValue)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Fervour
                var penitentFervour = penitent.Stats.Fervour;
                current = penitentFervour.Current;
                max = penitentFervour.CurrentMax;
                baseValue = penitentFervour.Base;
                upgradeCount = penitentFervour.GetUpgrades();
                upgradeValue = Traverse.Create(penitentFervour).Field("_upgradeValue").GetValue<float>();
                bonus = max - baseValue - upgradeCount * upgradeValue;
                sb.AppendLine($"Fervour: {RoundFixedPoint(current)}/{RoundFixedPoint(max)}");
                sb.AppendLine($"Max Fervour: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeValue)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Flask count
                var penitentFlaskCount = penitent.Stats.Flask;
                current = penitentFlaskCount.Current;
                max = penitentFlaskCount.CurrentMax;
                baseValue = penitentFlaskCount.Base;
                upgradeCount = penitentFlaskCount.GetUpgrades();
                upgradeValue = Traverse.Create(penitentFlaskCount).Field("_upgradeValue").GetValue<float>();
                bonus = max - baseValue - upgradeCount * upgradeValue;
                sb.AppendLine($"Flask count: {(int)current}/{(int)(max)}");
                sb.AppendLine($"Max Flasks: {(int)(max)} = {(int)(baseValue)}(base) + {upgradeCount}(upgrades) + {(int)(bonus)}(bonus)");

                // Flask heal amount
                var penitentFlaskHealAmount = penitent.Stats.FlaskHealth;
                max = penitentFlaskHealAmount.Final;
                baseValue = penitentFlaskHealAmount.Base;
                upgradeCount = penitentFlaskHealAmount.GetUpgrades();
                upgradeValue = Traverse.Create(penitentFlaskHealAmount).Field("_upgradeValue").GetValue<float>();
                bonus = max - baseValue - upgradeCount * upgradeValue;
                sb.AppendLine($"Flask heal amount: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeValue)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                break;
            case DisplayType.Defenses:
                float physicalDefense = penitent.Stats.NormalDmgReduction.Final;
                float contactDefense = penitent.Stats.ContactDmgReduction.Final;
                float magicDefense = penitent.Stats.MagicDmgReduction.Final;
                float lightningDefense = penitent.Stats.LightningDmgReduction.Final;
                float fireDefense = penitent.Stats.FireDmgReduction.Final;
                float toxicDefense = penitent.Stats.ToxicDmgReduction.Final;
                float globalDefense = penitent.Stats.Defense.Final;
                sb.AppendLine($"Damage reduction: ");
                sb.AppendLine($"- Physical: {RoundPercentage(physicalDefense)}");
                sb.AppendLine($"- Contact: {RoundPercentage(contactDefense)}");
                sb.AppendLine($"- Magic: {RoundPercentage(magicDefense)}");
                sb.AppendLine($"- Lightning: {RoundPercentage(lightningDefense)}");
                sb.AppendLine($"- Fire: {RoundPercentage(fireDefense)}");
                sb.AppendLine($"- Toxic: {RoundPercentage(toxicDefense)}");

                break;
            case DisplayType.Attacks:
                // Attack strength
                var penitentAttackStrength = penitent.Stats.Strength;
                max = penitentAttackStrength.Final;
                baseValue = penitentAttackStrength.Base;
                upgradeCount = penitentAttackStrength.GetUpgrades();
                upgradeValue = Traverse.Create(penitentAttackStrength).Field("_upgradeValue").GetValue<float>();
                bonus = max - baseValue - upgradeCount * upgradeValue;
                sb.AppendLine($"Attack damage: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeValue)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Fervour gain from attacks
                var penitentFervourStrength = penitent.Stats.FervourStrength;
                max = penitentFervourStrength.Final;
                baseValue = penitentFervourStrength.Base;
                bonus = max - baseValue;
                sb.AppendLine($"Fervour gain: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                // Prayer strength
                var penitentPrayerStrength = penitent.Stats.PrayerStrengthMultiplier;
                max = penitentPrayerStrength.Final;
                baseValue = penitentPrayerStrength.Base;
                bonus = max - baseValue;
                sb.AppendLine($"Prayer strength: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                // Bleeding Miracle strength
                var penitentRangedStrength = penitent.Stats.RangedStrength;
                max = penitentRangedStrength.Final;
                baseValue = penitentRangedStrength.Base;
                bonus = max - baseValue;
                sb.AppendLine($"Ranged strength: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                break;
            case DisplayType.Positional:
                // Scene
                string currentScene = SceneHelper.CurrentScene;
                sb.AppendLine($"Scene: {currentScene}");

                // Position
                Vector2 playerPosition = Core.Logic.Penitent.transform.position;
                sb.AppendLine($"Position: ({RoundFixedPoint(playerPosition.x)}, {RoundFixedPoint(playerPosition.y)})");

                // Velocity and speed
                Vector2 currentVelocity = Core.Logic.Penitent.PlatformCharacterController.InstantVelocity;
                float currentSpeed = currentVelocity.magnitude;
                float maxWalkingSpeed = Core.Logic.Penitent.PlatformCharacterController.MaxWalkingSpeed;
                sb.AppendLine($"Velocity: ({RoundFixedPoint(currentVelocity.x)}, {RoundFixedPoint(currentVelocity.y)})");
                sb.AppendLine($"Current speed: {RoundFixedPoint(currentSpeed)}");
                sb.AppendLine($"Max horizontal walk speed: {maxWalkingSpeed}");
                break;
        }

        infoText.text = sb.ToString();
    }

}
