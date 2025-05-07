using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Damage;
using HarmonyLib;

namespace Blasphemous.DebugMod.Events;


[HarmonyPatch(typeof(PenitentDamageArea), "RaiseDamageEvent")]
class Penitent_Damage_Patch
{
    public static void Postfix(ref Hit hit) => Main.Debugger.EventHandler.DamagePlayer(ref hit);
}

[HarmonyPatch(typeof(EnemyDamageArea), "TakeDamageAmount")]
class Enemy_Damage_Patch
{
    public static void Postfix(EnemyDamageArea __instance)
    {
        Enemy enemy = Traverse.Create(__instance).Field("_enemyEntity").GetValue<Enemy>();
        Main.Debugger.EventHandler.DamageEnemy(enemy);
    }
}
