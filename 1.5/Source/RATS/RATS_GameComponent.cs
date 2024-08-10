﻿using System.Collections.Generic;
using Verse;

namespace RATS;

public class RATS_GameComponent(Game game) : GameComponent
{
    public static Dictionary<Pawn, RATSAction> ActiveAttacks = new Dictionary<Pawn, RATSAction>();

    public static bool SlowMoActive;
    public static int SlowMoStarted = -1;
    public static Thing SlowMoCauser;
    public Game Game = game;

    public static void SetSlowMo(Thing slowMoCauser)
    {
        if (!RATSMod.Settings.EnableSlowDownTime)
        {
            return;
        }

        SlowMoStarted = Current.Game.tickManager.TicksGame;
        SlowMoActive = true;
        SlowMoCauser = slowMoCauser;
    }

    public static void ResetSlowMo()
    {
        SlowMoStarted = -1;
        SlowMoActive = false;
        SlowMoCauser = null;
    }

    public override void ExposeData()
    {
        ActiveAttacks ??= new Dictionary<Pawn, RATSAction>();
        Scribe_Collections.Look(ref ActiveAttacks, "ActiveAttacks", LookMode.Reference, LookMode.Reference);
    }

    public override void GameComponentTick()
    {
        // Safety barrier to prevent getting stuck in slowmo
        if (SlowMoCauser == null || SlowMoCauser.Destroyed || (SlowMoStarted > 0 && SlowMoStarted + 600 < Current.Game.tickManager.TicksGame))
        {
            ResetSlowMo();
        }
    }

    public class RATSAction(Pawn p, BodyPartRecord b, ThingWithComps t, float chance, ShotReport shotReport)
    {
        public ThingWithComps Equipment = t;
        public float HitChance = chance;
        public BodyPartRecord Part = b;
        public ShotReport ShotReport = shotReport;
        public Pawn Target = p;
    }
}
