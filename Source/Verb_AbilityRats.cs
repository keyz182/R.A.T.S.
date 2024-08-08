using System;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RATS;

public class Verb_AbilityRats : Verb_AbilityShoot, IAbilityVerb
{
    public CompEquippable PrimaryWeaponEq => CasterPawn?.equipment?.PrimaryEq;
    public ThingWithComps PrimaryWeapon => CasterPawn?.equipment?.Primary;

    public VerbProperties PrimaryWeaponVerbProps => PrimaryWeapon?.def?.Verbs.FirstOrDefault();

    public override float EffectiveRange =>
        PrimaryWeapon == null ? 0f : PrimaryWeaponVerbProps.range;

    public void RATS_Selection(
        LocalTargetInfo target,
        BodyPartRecord part,
        float hitChance,
        ShotReport shotReport
    )
    {
        if (PrimaryWeaponVerbProps == null)
            return;
        Job job = JobMaker.MakeJob(RATS_DefOf.RATS_AttackHybrid);

        job.maxNumMeleeAttacks = 1;
        job.maxNumStaticAttacks = 1;
        string uniqueLoadId = CalculateUniqueLoadID(PrimaryWeaponEq, tool, maneuver);
        Verb verb = (Verb)Activator.CreateInstance(PrimaryWeaponVerbProps.verbClass);
        PrimaryWeaponEq.verbTracker.AllVerbs.Add(verb);
        verb.loadID = uniqueLoadId;
        verb.verbProps = PrimaryWeaponVerbProps;
        verb.verbTracker = PrimaryWeaponEq.verbTracker;
        verb.tool = tool;
        verb.maneuver = maneuver;
        verb.caster = caster;

        job.verbToUse = verb;
        job.targetA = caster;
        job.targetB = currentTarget = target;
        job.endIfCantShootInMelee = true;

        if (RATS_GameComponent.ActiveAttacks.ContainsKey(CasterPawn))
            RATS_GameComponent.ActiveAttacks.Remove(CasterPawn);
        RATS_GameComponent.ActiveAttacks.Add(
            CasterPawn,
            new RATS_GameComponent.RATSAction(
                currentTarget.Pawn,
                part,
                PrimaryWeapon,
                hitChance,
                shotReport
            )
        );

        ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);

        CasterPawn.jobs.TryTakeOrderedJob(job);
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        Find.WindowStack.Add(new Dialog_RATS(this, target));
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        verbProps.DrawRadiusRing(caster.Position);
        if (!target.IsValid)
            return;
        GenDraw.DrawTargetHighlight(target);
    }

    public override bool Available()
    {
        return true;
    }

    public ShotReport GetShotReport()
    {
        return ShotReport.HitReportFor(caster, this, currentTarget);
    }

    public Ability ability;
    public Ability Ability
    {
        get { return this.ability; }
        set { this.ability = value; }
    }
}
