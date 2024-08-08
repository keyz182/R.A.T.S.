using System.Collections.Generic;
using Verse;

namespace RATS;

public class RATS_GameComponent(Game game) : GameComponent
{
    public Game Game = game;
    public static Dictionary<Pawn, RATSAction> ActiveAttacks = new Dictionary<Pawn, RATSAction>();

    public bool SlowMoActive = false;
    public int SlowMoStarted = -1;

    public static void SetSlowMo()
    {
        Current.Game.GetComponent<RATS_GameComponent>().SlowMoStarted = Current
            .Game
            .tickManager
            .TicksGame;
        Current.Game.GetComponent<RATS_GameComponent>().SlowMoActive = true;
    }

    public static void ResetSlowMo()
    {
        Current.Game.GetComponent<RATS_GameComponent>().SlowMoStarted = -1;
        Current.Game.GetComponent<RATS_GameComponent>().SlowMoActive = false;
    }

    public static float TickRateMultiplier()
    {
        RATS_GameComponent gc = Current.Game.GetComponent<RATS_GameComponent>();

        if (gc == null || !gc.SlowMoActive)
        {
            return Find.TickManager.TickRateMultiplier;
        }

        return 0.25f;
    }

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

    public override void GameComponentTick()
    {
        // Safety barrier to prevent getting stuck in slowmo
        if (SlowMoStarted > 0 && SlowMoStarted + 600 < Current.Game.tickManager.TicksGame)
        {
            SlowMoStarted = -1;
            SlowMoActive = false;
        }
    }
}
