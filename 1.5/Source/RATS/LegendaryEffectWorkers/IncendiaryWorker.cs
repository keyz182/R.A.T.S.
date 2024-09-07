using RimWorld;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class IncendiaryWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo)
    {
        if (damageInfo.IntendedTarget != null && damageInfo.IntendedTarget.CanEverAttachFire())
        {
            damageInfo.IntendedTarget.TryAttachFire(2, damageInfo.Instigator);
        }
    }
}
