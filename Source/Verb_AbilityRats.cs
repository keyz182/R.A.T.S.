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
    public ThingWithComps PrimaryWeapon => CasterPawn?.equipment?.Primary;
    
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

    public ThingDef TryGetProjectile()
    {
      CompChangeableProjectile comp = PrimaryWeapon?.GetComp<CompChangeableProjectile>();
      return comp != null && comp.Loaded ? comp.Projectile : PrimaryWeaponShootVerbProps.defaultProjectile;
    }

    public ShotReport GetShotReport()
    {
      return ShotReport.HitReportFor(this.caster, (Verb) this, this.currentTarget);
    }
    //
    // protected IntVec3 GetForcedMissTarget_New(float forcedMissRadius)
    // {
    //   if (PrimaryWeaponShootVerbProps.forcedMissEvenDispersal)
    //   {
    //     if (this.forcedMissTargetEvenDispersalCache.Count <= 0)
    //     {
    //       this.forcedMissTargetEvenDispersalCache.AddRange(Verb_LaunchProjectile.GenerateEvenDispersalForcedMissTargets(this.currentTarget.Cell, forcedMissRadius, this.burstShotsLeft));
    //       this.forcedMissTargetEvenDispersalCache.SortByDescending<IntVec3, int>((Func<IntVec3, int>) (p => p.DistanceToSquared(this.Caster.Position)));
    //     }
    //     if (this.forcedMissTargetEvenDispersalCache.Count > 0)
    //       return this.forcedMissTargetEvenDispersalCache.Pop<IntVec3>();
    //   }
    //   int index = Rand.Range(0, GenRadial.NumCellsInRadius(forcedMissRadius));
    //   return this.currentTarget.Cell + GenRadial.RadialPattern[index];
    // }
    //
    // public float CalculateForcedMiss()
    // {
    //   float forcedMissRadius = PrimaryWeaponShootVerbProps.ForcedMissRadius;
    //   forcedMissRadius *= PrimaryWeaponShootVerbProps.GetForceMissFactorFor(PrimaryWeapon, (Pawn)this.caster);
    //   return VerbUtility.CalculateAdjustedForcedMiss(forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
    // }
    //
    // public override bool TryCastShot()
    // {
    //   if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
    //     return false;
    //   ThingDef projectile1 = TryGetProjectile();
    //   if (projectile1 == null)
    //     return false;
    //   ShootLine resultingLine;
    //   bool shootLineFromTo = this.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out resultingLine);
    //   if (PrimaryWeaponShootVerbProps.stopBurstWithoutLos && !shootLineFromTo)
    //     return false;
    //   if (PrimaryWeapon != null)
    //   {
    //     PrimaryWeapon.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
    //     PrimaryWeapon.GetComp<CompApparelVerbOwner_Charged>()?.UsedOnce();
    //   }
    //   this.lastShotTick = Find.TickManager.TicksGame;
    //   Thing launcher = this.caster;
    //   Thing equipment = (Thing) PrimaryWeapon;
    //   CompMannable comp = this.caster.TryGetComp<CompMannable>();
    //   Vector3 drawPos = this.caster.DrawPos;
    //   Verse.Projectile projectile2 = (Verse.Projectile) GenSpawn.Spawn(projectile1, resultingLine.Source, this.caster.Map);
    //   
    //   
    //   if ((double) PrimaryWeaponShootVerbProps.ForcedMissRadius > 0.5)
    //   {
    //     float forcedMissRadius = this.verbProps.ForcedMissRadius;
    //     if (launcher is Pawn caster)
    //       forcedMissRadius *= this.verbProps.GetForceMissFactorFor(equipment, caster);
    //     float adjustedForcedMiss = VerbUtility.CalculateAdjustedForcedMiss(forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
    //     if ((double) adjustedForcedMiss > 0.5)
    //     {
    //       IntVec3 forcedMissTarget = this.GetForcedMissTarget(adjustedForcedMiss);
    //       if (forcedMissTarget != this.currentTarget.Cell)
    //       {
    //         this.ThrowDebugText("ToRadius");
    //         this.ThrowDebugText("Rad\nDest", forcedMissTarget);
    //         ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
    //         if (Rand.Chance(0.5f))
    //           hitFlags = ProjectileHitFlags.All;
    //         if (!this.canHitNonTargetPawnsNow)
    //           hitFlags &= ~ProjectileHitFlags.NonTargetPawns;
    //         projectile2.Launch(launcher, drawPos, (LocalTargetInfo) forcedMissTarget, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment);
    //         return true;
    //       }
    //     }
    //   }
    //   
    //   ShotReport shotReport = ShotReport.HitReportFor(this.caster, (Verb) this, this.currentTarget);
    //   Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
    //   ThingDef def = randomCoverToMissInto?.def;
    //   if (this.verbProps.canGoWild && !Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
    //   {
    //     bool flyOverhead = projectile2?.def?.projectile != null && projectile2.def.projectile.flyOverhead;
    //     resultingLine.ChangeDestToMissWild_NewTemp(shotReport.AimOnTargetChance_StandardTarget, flyOverhead, this.caster.Map);
    //     this.ThrowDebugText("ToWild" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
    //     this.ThrowDebugText("Wild\nDest", resultingLine.Dest);
    //     ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
    //     if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
    //       hitFlags |= ProjectileHitFlags.NonTargetPawns;
    //     projectile2.Launch(launcher, drawPos, (LocalTargetInfo) resultingLine.Dest, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment, def);
    //     return true;
    //   }
    //   if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.CanBenefitFromCover && !Rand.Chance(shotReport.PassCoverChance))
    //   {
    //     this.ThrowDebugText("ToCover" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
    //     this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
    //     ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
    //     if (this.canHitNonTargetPawnsNow)
    //       hitFlags |= ProjectileHitFlags.NonTargetPawns;
    //     projectile2.Launch(launcher, drawPos, (LocalTargetInfo) randomCoverToMissInto, this.currentTarget, hitFlags, this.preventFriendlyFire, equipment, def);
    //     return true;
    //   }
    //   ProjectileHitFlags hitFlags1 = ProjectileHitFlags.IntendedTarget;
    //   if (this.canHitNonTargetPawnsNow)
    //     hitFlags1 |= ProjectileHitFlags.NonTargetPawns;
    //   if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
    //     hitFlags1 |= ProjectileHitFlags.NonTargetWorld;
    //   this.ThrowDebugText("ToHit" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
    //   if (this.currentTarget.Thing != null)
    //   {
    //     projectile2.Launch(launcher, drawPos, this.currentTarget, this.currentTarget, hitFlags1, this.preventFriendlyFire, equipment, def);
    //     this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
    //   }
    //   else
    //   {
    //     projectile2.Launch(launcher, drawPos, (LocalTargetInfo) resultingLine.Dest, this.currentTarget, hitFlags1, this.preventFriendlyFire, equipment, def);
    //     this.ThrowDebugText("Hit\nDest", resultingLine.Dest);
    //   }
    //   return true;
    // }
}