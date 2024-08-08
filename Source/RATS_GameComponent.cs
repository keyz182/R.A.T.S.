using System.Collections.Generic;
using Verse;

namespace RATS;

public class RATS_GameComponent(Game game) : GameComponent
{
    public Game Game = game;
    public static Dictionary<Pawn, RATSAction> ActiveAttacks = new Dictionary<Pawn, RATSAction>();

    public class RATSAction(
        Pawn p,
        BodyPartRecord b,
        ThingWithComps t,
        float chance,
        ShotReport shotReport
    )
    {
        public Pawn Target = p;
        public BodyPartRecord Part = b;
        public ThingWithComps Equipment = t;
        public float HitChance = chance;
        public ShotReport ShotReport = shotReport;
    }

    public override void ExposeData()
    {
        if (ActiveAttacks == null)
            ActiveAttacks = new Dictionary<Pawn, RATSAction>();
        Scribe_Collections.Look(
            ref ActiveAttacks,
            "ActiveAttacks",
            LookMode.Reference,
            LookMode.Reference
        );
    }
}
