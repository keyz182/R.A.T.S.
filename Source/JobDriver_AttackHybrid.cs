using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RATS;

public class JobDriver_AttackHybrid : JobDriver
{
  private bool startedIncapacitated = false;
  private bool hasAttacked = false;

  public override void ExposeData()
  {
    base.ExposeData();
    Scribe_Values.Look<bool>(ref this.startedIncapacitated, "startedIncapacitated");
  }
  
    public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

    public bool TryStartAttack(LocalTargetInfo targ)
    {
      if (this.pawn.stances.FullBodyBusy || this.pawn.WorkTagIsDisabled(WorkTags.Violent))
        return false;
      bool allowManualCastWeapons = !this.pawn.IsColonist;
      Verb attackVerb = this.pawn.TryGetAttackVerb(targ.Thing, allowManualCastWeapons);
      // attackVerb.currentTarget = attackVerb.verbProps.ai_RangedAlawaysShootGroundBelowTarget
      //   ? (LocalTargetInfo)targ.Cell
      //   : targ;
      // attackVerb.canHitNonTargetPawnsNow = true;
      // attackVerb.preventFriendlyFire = false;
      // attackVerb.nonInterruptingSelfCast = false;
      // attackVerb.currentDestination = LocalTargetInfo.Invalid;
      // attackVerb.state = VerbState.Bursting;
      //
      // return attackVerb != null && attac Verb.TryStartCastOn(attackVerb.verbProps.ai_RangedAlawaysShootGroundBelowTarget ? (LocalTargetInfo) targ.Cell : targ);
      
      ShootLine resultingLine;
      bool shootLineFromTo = attackVerb.TryFindShootLineFromTo(TargetThingA.Position, TargetThingB.Position, out resultingLine);

      var projectile1 = attackVerb.GetProjectile();
      var projectile2 = (Verse.Projectile) GenSpawn.Spawn(projectile1, resultingLine.Source, TargetThingA.Map);

      var attack = RATS_GameComponent.ActiveAttacks.GetValueOrDefault(pawn);

      if (attack == null)
        return false;
      
      if (!Rand.Chance(attack.HitChance))
      {
        return false;
      }
      ProjectileHitFlags hitFlags1 = ProjectileHitFlags.IntendedTarget;
      
      projectile2.Launch(pawn, pawn.DrawPos, this.TargetB, this.TargetB, hitFlags1, false, attack.Equipment, null);
      return true;
    }
    
    public override IEnumerable<Toil> MakeNewToils()
    {
      yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
      Toil init = ToilMaker.MakeToil(nameof (MakeNewToils));
      init.initAction = (Action) (() =>
      {
        if (this.TargetThingA is Pawn targetThingA2)
          this.startedIncapacitated = targetThingA2.Downed;
        this.pawn.pather.StopDead();
      });
      init.tickAction = (Action) (() =>
      {
        if (!this.TargetA.IsValid)
        {
          this.EndJobWith(JobCondition.Succeeded);
        }
        else
        {
          if (this.TargetA.HasThing)
          {
            Pawn thing = this.TargetA.Thing as Pawn;
            if (this.TargetA.Thing.Destroyed || thing != null && !this.startedIncapacitated && thing.Downed || thing != null && thing.IsPsychologicallyInvisible())
            {
              this.EndJobWith(JobCondition.Succeeded);
              return;
            }
          }
          if (hasAttacked)
            this.EndJobWith(JobCondition.Succeeded);
          else if (TryStartAttack(this.TargetA))
          {
            hasAttacked = true;
          }
          else
          {
            if (this.pawn.stances.FullBodyBusy)
              return;
            Verb attackVerb = this.pawn.TryGetAttackVerb(this.TargetA.Thing, !this.pawn.IsColonist);
            if (this.job.endIfCantShootTargetFromCurPos && (attackVerb == null || !attackVerb.CanHitTargetFrom(this.pawn.Position, this.TargetA)))
            {
              this.EndJobWith(JobCondition.Incompletable);
            }
            else
            {
              if (!this.job.endIfCantShootInMelee)
                return;
              if (attackVerb == null)
              {
                this.EndJobWith(JobCondition.Incompletable);
              }
              else
              {
                float num = attackVerb.verbProps.EffectiveMinRange(this.TargetA, (Thing) this.pawn);
                IntVec3 position1 = this.pawn.Position;
                LocalTargetInfo targetA = this.TargetA;
                IntVec3 cell1 = targetA.Cell;
                if ((double) position1.DistanceToSquared(cell1) >= (double) num * (double) num)
                  return;
                IntVec3 position2 = this.pawn.Position;
                targetA = this.TargetA;
                IntVec3 cell2 = targetA.Cell;
                if (!position2.AdjacentTo8WayOrInside(cell2))
                  return;
                this.EndJobWith(JobCondition.Incompletable);
              }
            }
          }
        }
      });
      init.defaultCompleteMode = ToilCompleteMode.Never;
      init.activeSkill = (Func<SkillDef>) (() => Toils_Combat.GetActiveSkillForToil(init));
      yield return init;
    }
}