using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class Command_RATS : Command_VerbTarget
{
    public Ability Ability;
    public Verb_AbilityRats Verb => (Verb_AbilityRats)Ability.verb;
    public Pawn Pawn;
    
    public override bool Visible
    {
        get
        {
            if (this.Verb?.PrimaryWeaponVerbProps == null)
            {
                return false;
            }

            return !this.Verb.PrimaryWeaponVerbProps.IsMeleeAttack;
        }
    }

    public Command_RATS(Ability ability, Pawn pawn)
    {
        Ability = ability;
        Pawn = pawn;
        verb = Ability.verb;
    }

    public override Color IconDrawColor => defaultIconColor;


    public override void GizmoUpdateOnMouseover()
    {
        if (!drawRadius)
            return;
        verb.verbProps.DrawRadiusRing_NewTemp(verb.caster.Position, verb);
    }
}