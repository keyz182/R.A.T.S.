using RimWorld;
using Verse;

namespace RATS;

public class HediffComp_CripplePart : HediffComp
{
    public HediffCompProperties_CripplePart Props => (HediffCompProperties_CripplePart)props;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        BodyPartRecord part = parent.Part;
        if (part == null)
            return;

        Pawn.TakeDamage(new DamageInfo(Props.damageDef, 0, hitPart: part));
        Messages.Message("RATS_MessageReceivedDamageFromHediff".Translate(Pawn.Named("PAWN"), part.LabelCap), (Thing)Pawn, MessageTypeDefOf.NegativeEvent);
    }

    public override void CompPostPostRemoved() { }
}
