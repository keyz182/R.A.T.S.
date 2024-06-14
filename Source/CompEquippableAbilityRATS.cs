using RimWorld;
using Verse;

namespace RATS;

public class CompEquippableAbilityRATS : CompEquippableAbility
{
    public CompProperties_EquippableAbilityRATS Props
    {
        get => this.props as CompProperties_EquippableAbilityRATS;
    }
    
    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
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