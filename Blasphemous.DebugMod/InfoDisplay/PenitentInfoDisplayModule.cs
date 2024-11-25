using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Attribute = Framework.FrameworkCore.Attributes.Logic.Attribute;

namespace Blasphemous.DebugMod.InfoDisplay;

/// <summary>
/// Module for displaying penitent information
/// </summary>
internal class PenitentInfoDisplayModule(int precision) : BaseModule(keybindName, false)
{
    private readonly int _precision = precision;
    internal static readonly string gameObjectName = "Penitent info display";
    internal static readonly string keybindName = "Penitent_Info_Display";

    private GameObject _gameObject;
    private Text _infoText;
    private DisplayType _currentDisplayType;

    private enum DisplayType
    {
        HealthFervourFlask,
        Defenses,
        Attacks,
        Prayer,
        Positional,
        Misc
    }

    protected override void OnActivate()
    {
        if (_gameObject == null)
            CreateGameObject();
        else if (_gameObject?.GetComponentInChildren<Text>() == null)
            CreateGameObject();

        _gameObject?.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        _gameObject?.SetActive(false);
    }

    public override void Update()
    {
        if (Main.Debugger.InputHandler.GetKeyDown(_input))
        {
            ModLog.Info($"Toggling {_input}");
            if (IsActive == false)
            {
                IsActive = true;
                _currentDisplayType = 0;
                ModLog.Info($"Turning on {gameObjectName}");
                ModLog.Info($"Displaying {_currentDisplayType}");
            }
            else
            {
                // switch to the next display option
                if (_currentDisplayType == (DisplayType)Enum.GetValues(typeof(DisplayType)).Cast<int>().Max())
                {
                    // when reaching the last display option, deactivate display at toggle
                    IsActive = false;
                    ModLog.Info($"Turning off {gameObjectName}");
                }
                else
                {
                    _currentDisplayType++;
                    ModLog.Info($"Displaying {_currentDisplayType}");
                }
            }
        }

        OnUpdate();
    }

    protected override void OnUpdate()
    {
        if (!IsActive)
            return;

        UpdateText(_currentDisplayType);
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

    private void CreateGameObject()
    {
        Transform parent = UnityEngine.Object.FindObjectsOfType<PlayerFervour>().FirstOrDefault(x => x.name == "Fervour Bar")?.transform;
        if (parent == null)
            return;

        Vector2 rectSize = new Vector2(250, 250);

        _infoText = UIModder.Create(new RectCreationOptions()
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
        GameObject obj = _infoText.gameObject;
        GameObject spriteObject = new($"{gameObjectName}_sprite");
        spriteObject.transform.SetParent(obj.transform, false);

        /* WIP
        // create a 1*1 sprite for coloring use
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);

        // add sprite to the UI gameObject
        spriteObject.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 10000;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0, 1));
        sr.color = new Color(0, 0, 0, 0.7f);
        spriteObject.transform.localScale = new Vector3(rectSize.x, rectSize.y, 1);
        */

        // store this gameObject to a field of this class
        _gameObject = obj;
    }

    private void UpdateText(DisplayType displayType)
    {
        var sb = new StringBuilder();
        var penitent = Core.Logic.Penitent;

        if (penitent == null)
        {
            sb.AppendLine($"Penitent info N/A");
            return;
        }

        float current;
        float max;
        float baseValue;
        int upgradeCount;
        float upgradeIncrement;
        float bonus;

        switch (displayType)
        {
            case DisplayType.HealthFervourFlask:
                // Health
                GetAttributeDetails(penitent.Stats.Life,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Health: {RoundFixedPoint(current)}/{RoundFixedPoint(max)}");
                sb.AppendLine($"Max Health: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeIncrement)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Fervour
                GetAttributeDetails(penitent.Stats.Fervour,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Fervour: {RoundFixedPoint(current)}/{RoundFixedPoint(max)}");
                sb.AppendLine($"Max Fervour: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeIncrement)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Flask count
                GetAttributeDetails(penitent.Stats.Flask,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Flask count: {(int)current}/{(int)(max)}");
                sb.AppendLine($"Max Flasks: {(int)(max)} = {(int)(baseValue)}(base) + {upgradeCount}(upgrades) + {(int)(bonus)}(bonus)");

                // Flask heal amount
                GetAttributeDetails(penitent.Stats.FlaskHealth,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Flask heal amount: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeIncrement)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                break;
            case DisplayType.Defenses:
                float physicalDefense = penitent.Stats.NormalDmgReduction.Final;
                float contactDefense = penitent.Stats.ContactDmgReduction.Final;
                float magicDefense = penitent.Stats.MagicDmgReduction.Final;
                float lightningDefense = penitent.Stats.LightningDmgReduction.Final;
                float fireDefense = penitent.Stats.FireDmgReduction.Final;
                float toxicDefense = penitent.Stats.ToxicDmgReduction.Final;
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
                GetAttributeDetails(penitent.Stats.Strength,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Attack damage: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {upgradeCount}*{RoundFixedPoint(upgradeIncrement)}(upgrades) + {RoundFixedPoint(bonus)}(bonus)");

                // Attack speed
                GetAttributeDetails(penitent.Stats.AttackSpeed,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Attack speed: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                // Fervour gain from attacks
                GetAttributeDetails(penitent.Stats.FervourStrength,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Fervour gain: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                // Bleeding Miracle strength
                GetAttributeDetails(penitent.Stats.RangedStrength,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Ranged damage: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                break;
            case DisplayType.Prayer:
                // Prayer strength
                GetAttributeDetails(penitent.Stats.PrayerStrengthMultiplier,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Prayer damage: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                // Prayer cost
                GetAttributeDetails(penitent.Stats.PrayerCostAddition,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                float currentPrayerBaseCost = Core.InventoryManager.GetPrayerInSlot(0).fervourNeeded;
                sb.AppendLine($"Current prayer cost: {RoundFixedPoint(currentPrayerBaseCost + max)} = {RoundFixedPoint(currentPrayerBaseCost)}(base) + {RoundFixedPoint(max)}(bonus)");

                // Prayer Duration
                GetAttributeDetails(penitent.Stats.PrayerDurationAddition,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                float currentPrayerBaseDuration = Core.InventoryManager.GetPrayerInSlot(0).EffectTime;
                if (currentPrayerBaseDuration <= Mathf.Epsilon)
                {
                    sb.AppendLine($"Current prayer duration: {currentPrayerBaseDuration} [cannot be elongated]");
                }
                else
                {
                    sb.AppendLine($"Current prayer duration: {RoundFixedPoint(currentPrayerBaseDuration + max)} = {RoundFixedPoint(currentPrayerBaseDuration)}(base) + {RoundFixedPoint(max)}(bonus)");
                }
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

                // Dash cooldown
                GetAttributeDetails(penitent.Stats.DashCooldown,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Dash cooldown: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                // Dash distance
                GetAttributeDetails(penitent.Stats.DashRide,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Dash distance: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                // Air impulse count: 
                GetAttributeDetails(penitent.Stats.AirImpulses,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Air impulses max: {(int)(max)} = {(int)(baseValue)}(base) + {(int)(bonus)}(bonus)");

                break;
            case DisplayType.Misc:
                // Tears of Atonement multiplier
                GetAttributeDetails(penitent.Stats.PurgeStrength,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Tears multiplier: {RoundPercentage(max)} = {RoundPercentage(baseValue)}(base) + {RoundPercentage(bonus)}(bonus)");

                // Parry window
                GetAttributeDetails(penitent.Stats.ParryWindow,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Parry window: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                // Righteous Riposte window
                GetAttributeDetails(penitent.Stats.ActiveRiposteWindow,
                    out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
                sb.AppendLine($"Righteous Riposte window: {RoundFixedPoint(max)} = {RoundFixedPoint(baseValue)}(base) + {RoundFixedPoint(bonus)}(bonus)");

                break;
        }

        _infoText.text = sb.ToString();
    }

    /// <summary>
    /// Outputs statistics about the attribute given.
    /// </summary>
    /// <param name="attribute">Input attribute.</param>
    /// <param name="current">Current value of the attribute.</param>
    /// <param name="max">Current max value of the attribute.</param>
    /// <param name="baseValue">Base value of the attribute.</param>
    /// <param name="upgradeCount">Number of times the attribute is upgraded.</param>
    /// <param name="upgradeIncrement">Increment on the attribute for each upgrade.</param>
    /// <param name="bonus">Sum of all other bonuses besides base and upgrades. bonus = max - baseValue - upgradeCount * upgradeIncrement </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if parameter `attribute` is not <see cref="Attribute"/> </exception>
    private static void GetAttributeDetails(
        object attribute,
        out float current,
        out float max,
        out float baseValue,
        out int upgradeCount,
        out float upgradeIncrement,
        out float bonus)
    {
        if (attribute is not Attribute)
        {
            throw new ArgumentOutOfRangeException($"{attribute} is not instance of `Attribute` or its subclasses!");
        }

        if (attribute is VariableAttribute)
        {
            VariableAttribute tempAttr = attribute as VariableAttribute;
            current = tempAttr.Current;
            max = tempAttr.CurrentMax;
        }
        else
        {
            Attribute tempAttr = attribute as Attribute;
            current = 0;
            max = tempAttr.Final;
        }

        Attribute attr = attribute as Attribute;
        baseValue = attr.Base;
        upgradeCount = attr.GetUpgrades();
        upgradeIncrement = Traverse.Create(attr).Field("_upgradeValue").GetValue<float>();
        bonus = max - baseValue - upgradeCount * upgradeIncrement;
    }

}
