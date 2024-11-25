using Gameplay.GameControllers.Entities;

namespace Blasphemous.DebugMod.Events;

internal class EventHandler
{
    public delegate void EventDelegate();

    public delegate void HitEvent(ref Hit hit);
    public delegate void EnemyEvent(Enemy enemy);

    public event HitEvent OnPlayerDamaged;
    public event EnemyEvent OnEnemyDamaged;

    public void DamagePlayer(ref Hit hit)
    {
        OnPlayerDamaged?.Invoke(ref hit);
    }

    public void DamageEnemy(Enemy enemy)
    {
        OnEnemyDamaged?.Invoke(enemy);
    }
}
