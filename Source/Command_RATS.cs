using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

[StaticConstructorOnStartup]
public class Command_RATS : Command_VerbTarget
{
    public Ability Ability;
    public Verb_AbilityRats Verb => (Verb_AbilityRats)Ability.verb;
    public Pawn Pawn;
    
    public static readonly Texture2D Tex = ContentFinder<Texture2D>.Get("UI/RATS_Logo-Small");
    
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
        icon = Tex;
    }

    public override Color IconDrawColor => defaultIconColor;


    public override void GizmoUpdateOnMouseover()
    {
        if (!drawRadius)
            return;
        verb.verbProps.DrawRadiusRing_NewTemp(verb.caster.Position, verb);
    }
}