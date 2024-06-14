using RimWorld;
using Verse;

namespace RATS;

public class CompEquippableAbilityRATS : CompEquippableAbility
{
    public override void Initialize(CompProperties inPprops)
    {
        base.Initialize(inPprops);
        if (this.Holder == null)
            return;
    }
    
    public override void Notify_Equipped(Pawn pawn)
    {
        Holder.abilities.GainAbility(Props.abilityDef);
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        Holder.abilities.RemoveAbility(Props.abilityDef);
    }
}