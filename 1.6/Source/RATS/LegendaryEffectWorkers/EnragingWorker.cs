using RimWorld;
using Verse;
using Verse.AI;

namespace RATS.LegendaryEffectWorkers;

public class EnragingWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo, Pawn pawn)
    {
        if (pawn != null)
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, damageInfo.Weapon.LabelCap, true, causedByDamage: true);
        }
    }
}
