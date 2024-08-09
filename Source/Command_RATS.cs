using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

[StaticConstructorOnStartup]
public class Command_RATS : Command_Ability
{
    public static readonly Texture2D Tex = ContentFinder<Texture2D>.Get("UI/RATS_Logo-Small");

    public Command_RATS(Ability ability, Pawn pawn)
        : base(ability, pawn)
    {
        icon = Tex;
        Verb.Ability = Ability;
    }

    public Verb_AbilityRats Verb => (Verb_AbilityRats)Ability.verb;

    public override bool Visible
    {
        get
        {
            if (Verb?.PrimaryWeaponVerbProps == null)
                return false;

            return !Verb.PrimaryWeaponVerbProps.IsMeleeAttack;
        }
    }

    public override Color IconDrawColor => defaultIconColor;

    public override void GizmoUpdateOnMouseover()
    {
        Verb.verbProps.DrawRadiusRing_NewTemp(Verb.caster.Position, Verb);
    }
}
