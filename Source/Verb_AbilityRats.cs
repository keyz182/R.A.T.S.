using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RATS;

public class Verb_AbilityRats : Verb_AbilityShoot
{
    protected bool ShowRATS = false;
    
    public Verb_AbilityRats() : base()
    {
    }

    public VerbProperties PrimaryWeaponShootVerbProps =>
        CasterPawn?.equipment?.Primary?.def?.verbs?.FirstOrDefault(v => v.verbClass.IsAssignableFrom(typeof(Verb_Shoot)));

    public CompEquippable PrimaryWeaponEq => CasterPawn?.equipment?.PrimaryEq;
    public override float EffectiveRange
    {
        get
        {
            if (!this.CasterPawn?.equipment?.Primary?.def?.IsRangedWeapon ?? false)
                return 0f;

            return PrimaryWeaponShootVerbProps?.range ?? this.verbProps.AdjustedRange(this, this.CasterPawn);
        }
    }

    public void RATS_Selection(LocalTargetInfo target, BodyPartRecord part)
    {
        if (PrimaryWeaponShootVerbProps != null)
        {
            Job job = JobMaker.MakeJob(PrimaryWeaponShootVerbProps.ai_IsWeapon ? JobDefOf.AttackStatic : JobDefOf.UseVerbOnThing);
            
            string uniqueLoadId = Verb.CalculateUniqueLoadID(PrimaryWeaponEq, tool, maneuver);
            Verb verb = (Verb) Activator.CreateInstance(PrimaryWeaponShootVerbProps.verbClass);
            PrimaryWeaponEq.verbTracker.verbs.Add(verb);
            verb.loadID = uniqueLoadId;
            verb.verbProps = PrimaryWeaponShootVerbProps;
            verb.verbTracker = PrimaryWeaponEq.verbTracker;
            verb.tool = tool;
            verb.maneuver = maneuver;
            verb.caster = caster;
            
            
            job.verbToUse = verb;
            job.targetA = target;
            job.endIfCantShootInMelee = true;
            this.CasterPawn.jobs.TryTakeOrderedJob(job);
        }
    }
    
    public override void OrderForceTarget(LocalTargetInfo target)
    {
        // Find.Selector.Deselect(this.CasterPawn);
        Find.WindowStack.Add((Window) new Dialog_RATS(this, target));
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
}