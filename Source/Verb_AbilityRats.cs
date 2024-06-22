using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace RATS;

public class Verb_AbilityRats : Verb_AbilityShoot
{
    public CompEquippable PrimaryWeaponEq => CasterPawn?.equipment?.PrimaryEq;
    public ThingWithComps PrimaryWeapon => CasterPawn?.equipment?.Primary;

    public VerbProperties PrimaryWeaponVerbProps =>
        PrimaryWeapon?.def?.verbs?.FirstOrDefault();

    public override float EffectiveRange => PrimaryWeapon == null ? 0f : PrimaryWeaponVerbProps.range;

    public void RATS_Selection(LocalTargetInfo target, BodyPartRecord part, float hitChance)
    {
        if (PrimaryWeaponVerbProps == null) return;
        Job job = JobMaker.MakeJob(RATS_DefOf.RATSAttackHybrid);

        job.maxNumMeleeAttacks = 1;
        job.maxNumStaticAttacks = 1;
        string uniqueLoadId = Verb.CalculateUniqueLoadID(PrimaryWeaponEq, tool, maneuver);
        Verb verb = (Verb)Activator.CreateInstance(PrimaryWeaponVerbProps.verbClass);
        PrimaryWeaponEq.verbTracker.verbs.Add(verb);
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
        RATS_GameComponent.ActiveAttacks.Add(CasterPawn,
            new RATS_GameComponent.RATSAction(currentTarget.Pawn, part, PrimaryWeapon, hitChance));

        CasterPawn.jobs.TryTakeOrderedJob(job);
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        Find.WindowStack.Add(new Dialog_RATS(this, target));
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        this.verbProps.DrawRadiusRing(this.caster.Position);
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
}