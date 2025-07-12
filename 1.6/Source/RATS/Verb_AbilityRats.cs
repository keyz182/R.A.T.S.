using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RATS;

public class Verb_AbilityRats : Verb_AbilityShoot
{
    public Ability ability;
    public CompEquippable PrimaryWeaponEq => CasterPawn?.equipment?.PrimaryEq;
    public ThingWithComps PrimaryWeapon => CasterPawn?.equipment?.Primary;

    public VerbProperties PrimaryWeaponVerbProps => PrimaryWeapon?.def?.Verbs.FirstOrDefault();

    public new Ability Ability
    {
        get => ability;
        set
        {
            AccessTools.Field(typeof(Ability), "cooldownDuration").SetValue(value, RATSMod.Settings.CooldownTicks);
            ability = value;
        }
    }

    public override float EffectiveRange => PrimaryWeapon == null ? 0f : PrimaryWeaponVerbProps.range;

    public void RATS_Selection(LocalTargetInfo target, BodyPartRecord part, float hitChance, ShotReport shotReport)
    {
        if (PrimaryWeaponVerbProps == null)
        {
            return;
        }

        Job job = JobMaker.MakeJob(RATS_DefOf.RATS_AttackHybrid);

        job.maxNumMeleeAttacks = 1;
        job.maxNumStaticAttacks = 1;

        Verb verb = PrimaryWeaponEq.verbTracker.PrimaryVerb;

        job.verbToUse = verb;
        job.targetA = caster;
        job.targetB = currentTarget = target;
        job.endIfCantShootInMelee = true;

        if (RATS_GameComponent.ActiveAttacks.ContainsKey(CasterPawn))
        {
            RATS_GameComponent.ActiveAttacks.Remove(CasterPawn);
        }

        RATS_GameComponent.ActiveAttacks.Add(CasterPawn, new RATS_GameComponent.RATSAction(currentTarget.Pawn, part, PrimaryWeapon, hitChance, shotReport));

        ability.StartCooldown(Mathf.CeilToInt(ability.def.cooldownTicksRange.RandomInRange * LegendaryEffectGameTracker.CooldownModifier(CasterPawn)));

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
        {
            return;
        }

        GenDraw.DrawTargetHighlight(target);
    }

    public override bool Available()
    {
        return true;
    }

    public ShotReport GetShotReport(LocalTargetInfo targetInfo)
    {
        return ShotReport.HitReportFor(caster, PrimaryWeaponEq.verbTracker.PrimaryVerb, targetInfo);
    }

    public override void OnGUI(LocalTargetInfo target)
    {
        CellRect occupiedRect = target.HasThing ? target.Thing.OccupiedRect() : CellRect.SingleCell(target.Cell);
        bool cannotShoot = !target.IsValid || OutOfRange(caster.Position, target, occupiedRect);
        Texture2D attachment =
            cannotShoot ? TexCommand.CannotShoot
            : !(UIIcon != BaseContent.BadTex) ? TexCommand.Attack
            : UIIcon;

        GenUI.DrawMouseAttachment(attachment);
    }

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        CellRect occupiedRect = target.HasThing ? target.Thing.OccupiedRect() : CellRect.SingleCell(target.Cell);
        return !OutOfRange(caster.Position, target, occupiedRect) && base.ValidateTarget(target, showMessages);
    }
}
