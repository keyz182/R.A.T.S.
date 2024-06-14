using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RATS;

public class RATSAbility : Ability
{
    public RATSAbility() : base() 
    {
    }
    public RATSAbility(Pawn pawn) : base(pawn) 
    {
    }
    public RATSAbility(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept)
    {
    }
    public RATSAbility(Pawn pawn, AbilityDef def) : base(pawn, def) 
    {
    }
    public RATSAbility(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def) 
    {
    }
}