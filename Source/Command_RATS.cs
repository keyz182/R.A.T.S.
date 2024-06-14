using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class Command_RATS: Command_VerbTarget
{
    public Ability Ability;
    public Pawn Pawn;
    
    public Command_RATS(Ability ability, Pawn pawn)
    {
        this.Ability = ability;
        this.Pawn = pawn;
        verb = this.Ability.verb;
    }

    public override Color IconDrawColor => base.defaultIconColor;

    
    public override void GizmoUpdateOnMouseover()
    {
        if (!this.drawRadius)
            return;
        this.verb.verbProps.DrawRadiusRing_NewTemp(this.verb.caster.Position, this.verb);
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
    }
}