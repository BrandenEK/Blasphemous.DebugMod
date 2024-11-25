using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Framework.EditorScripts.BossesBalance;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Attribute = Framework.FrameworkCore.Attributes.Logic.Attribute;

namespace Blasphemous.DebugMod.InfoDisplay;

/// <summary>
/// Module for displaying enemy information
/// </summary>
internal class EnemyInfoDisplayModule(int precision) : BaseModule(keybindName, false)
{
    private readonly int _precision = precision;
    internal static readonly string gameObjectName = "Enemy info display";
    internal static readonly string keybindName = "Enemy_Info_Display";

    private GameObject _gameObject;
    private Text _infoText;
    private DisplayType _currentDisplayType;
    private GameObject _enemyPlayerLastHit;
    private GameObject _enemyLastAttackedPlayer;
    private bool _eventRegistered = false;
    private string _infoPlayerLastHit = "Last attacked:\n- N/A";
    private string _infoLastAttackedPlayer = "Last damaged by:\n- N/A";


    private enum DisplayType
    {
        PlayerLastHit,
        LastAttackedPlayer
    }

    protected override void OnActivate()
    {
        if (_gameObject == null)
            CreateGameObject();
        else if (_gameObject?.GetComponentInChildren<Text>() == null)
            CreateGameObject();

        if (!_eventRegistered)
        {
            _eventRegistered = true;
            Main.Debugger.EventHandler.OnEnemyDamaged += OnEnemyHit;
            Main.Debugger.EventHandler.OnPlayerDamaged += OnPlayerHit;
        }

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
                ShowText(_currentDisplayType);

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
                    ShowText(_currentDisplayType);

                    ModLog.Info($"Displaying {_currentDisplayType}");
                }
            }
        }

        OnUpdate();
    }

    protected override void OnUpdate()
    {

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
            Position = new Vector2(400, -45),
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

        // store this gameObject to mod prefabStorage and a field of this class
        _gameObject = obj;
    }

    private void UpdateText(DisplayType displayType)
    {
        var sb = new StringBuilder();
        GameObject obj = null;
        Enemy enemy = null;

        float current;
        float max;
        float baseValue;
        int upgradeCount;
        float upgradeIncrement;
        float bonus;

        switch (displayType)
        {
            case DisplayType.PlayerLastHit:
                obj = _enemyPlayerLastHit;
                sb.AppendLine($"Last attacked:");
                break;
            case DisplayType.LastAttackedPlayer:
                obj = _enemyLastAttackedPlayer;
                sb.AppendLine($"Last damaged by:");
                break;
        }

        if (obj == null)
        {
            sb.AppendLine($"-  N/A");
            _infoText.text = sb.ToString();
            return;
        }

        enemy = obj?.GetComponentInChildren<Enemy>();
        if (enemy == null)
        {
            sb.AppendLine($"-  N/A");
            _infoText.text = sb.ToString();
            return;
        }

        EntityStats enemyStats = Traverse.Create(enemy).Field("Stats").GetValue<EntityStats>();

        // Name
        sb.AppendLine($"[{enemy.Id}] {enemy.EntityName}");

        // Health
        GetAttributeDetails(enemyStats.Life,
            out current, out max, out baseValue, out upgradeCount, out upgradeIncrement, out bonus);
        sb.AppendLine($"Health: {RoundFixedPoint(current)}/{RoundFixedPoint(max)}");

        // Damage
        // Distinguish if the enemy is a boss or a regular mob, and get the corresponding balance sheet item.
        if (enemy.Id.StartsWith("BS")) // Is boss
        {
            BossesBalanceChart bossesBalanceChart = Core.GameModeManager.GetCurrentBossesBalanceChart();
            List<Dictionary<string, object>> bossesBalance = bossesBalanceChart.BossesBalance;
            foreach (var bossDict in bossesBalance)
            {
                if (!bossDict.TryGetValue("Id", out object temp))
                    continue;

                string bossId = temp.ToString();
                if (enemy.Id.Equals(bossId))
                {
                    Dictionary<string, object> bossBalanceStats = bossDict;
                    sb.AppendLine($"Light attack damage: {RoundFixedPoint(float.Parse(bossBalanceStats["Light Attack"].ToString()))}");
                    sb.AppendLine($"Medium attack damage: {RoundFixedPoint(float.Parse(bossBalanceStats["Medium Attack"].ToString()))}");
                    sb.AppendLine($"Heavy attack damage: {RoundFixedPoint(float.Parse(bossBalanceStats["Heavy Attack"].ToString()))}");
                    sb.AppendLine($"Critical attack damage: {RoundFixedPoint(float.Parse(bossBalanceStats["Critical Attack"].ToString()))}");
                    sb.AppendLine($"Contact damage: {RoundFixedPoint(float.Parse(bossBalanceStats["Contact Damage"].ToString()))}");

                    break;
                }
            }
        }
        else // Is regular mob
        {
            EnemyBalanceItem enemyBalanceStats = Core.GameModeManager.GetCurrentEnemiesBalanceChart().EnemiesBalanceItems.First(x => x.Id.Equals(enemy.Id));
            sb.AppendLine($"Attack damage: {RoundFixedPoint(enemyBalanceStats.Strength)}");
            sb.AppendLine($"Contact damage: {RoundFixedPoint(enemyBalanceStats.ContactDamage)}");
        }

        // Tears of Atonement
        sb.AppendLine($"Tears of atonement drop: {(int)enemy.purgePointsWhenDead}");

        // Defenses
        float physicalDR = enemyStats.NormalDmgReduction.Final;
        float contactDR = enemyStats.ContactDmgReduction.Final;
        float magicDR = enemyStats.MagicDmgReduction.Final;
        float lightningDR = enemyStats.LightningDmgReduction.Final;
        float fireDR = enemyStats.FireDmgReduction.Final;
        float toxicDR = enemyStats.ToxicDmgReduction.Final;
        Dictionary<string, float> defenses = new()
        {
            { "Physical", physicalDR },
            { "Contact", contactDR },
            { "Magic", magicDR },
            { "Lightning", lightningDR },
            { "Fire", fireDR },
            { "Toxic", toxicDR },
        };

        sb.AppendLine($"Damage reduction: ");
        if (defenses.Values.Sum() > Mathf.Epsilon)
        {
            foreach (var kvp in defenses)
            {
                if (kvp.Value > Mathf.Epsilon)
                {
                    sb.AppendLine($"- {kvp.Key}: {RoundPercentage(kvp.Value)}");
                }
            }
        }
        else
        {
            sb.AppendLine($"- No damage reduction to any element!");
        }

        // store the string to corresponding string storage
        if (displayType == DisplayType.PlayerLastHit)
        {
            _infoPlayerLastHit = sb.ToString();
        }
        else if (displayType == DisplayType.LastAttackedPlayer)
        {
            _infoLastAttackedPlayer = sb.ToString();
        }
    }

    private void ShowText(DisplayType displayType)
    {
        if (displayType == DisplayType.PlayerLastHit)
        {
            _infoText.text = _infoPlayerLastHit;
        }
        else if (displayType == DisplayType.LastAttackedPlayer)
        {
            _infoText.text = _infoLastAttackedPlayer;
        }
    }

    private void OnPlayerHit(ref Hit hit)
    {
        _enemyLastAttackedPlayer = hit.AttackingEntity;
        UpdateText(DisplayType.LastAttackedPlayer);
        if (_currentDisplayType == DisplayType.LastAttackedPlayer)
        {
            ShowText(_currentDisplayType);
        }
    }

    private void OnEnemyHit(Enemy enemy)
    {
        _enemyPlayerLastHit = enemy.gameObject;
        UpdateText(DisplayType.PlayerLastHit);
        if (_currentDisplayType == DisplayType.PlayerLastHit)
        {
            ShowText(_currentDisplayType);
        }
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
