using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RATS;

public class JobDriver_AttackHybrid : JobDriver
{
    private bool startedIncapacitated;
    private bool hasAttacked;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref startedIncapacitated, "startedIncapacitated");
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

    public bool TryStartAttack(LocalTargetInfo targ)
    {
        if (pawn.stances.FullBodyBusy || pawn.WorkTagIsDisabled(WorkTags.Violent))
            return false;
        bool allowManualCastWeapons = !pawn.IsColonist;
        Verb attackVerb = pawn.TryGetAttackVerb(targ.Thing, allowManualCastWeapons);

        ShootLine resultingLine;
        bool shootLineFromTo = attackVerb.TryFindShootLineFromTo(
            TargetThingA.Position,
            TargetThingB.Position,
            out resultingLine
        );

        ThingDef projectile1 = attackVerb.GetProjectile();
        Projectile projectile2 = (Projectile)
            GenSpawn.Spawn(projectile1, resultingLine.Source, TargetThingA.Map);

        if (
            !RATS_GameComponent.ActiveAttacks.TryGetValue(
                pawn,
                out RATS_GameComponent.RATSAction attack
            )
        )
            return false;

        LocalTargetInfo hitTarget = null;
        if (!Rand.Chance(attack.HitChance))
        {
            Log.Message(
                $"RATS attack Missed to {attack.Target} on {attack.Part} with hit chance {attack.HitChance}"
            );

            int missDir = Rand.Range(0, 7);

            IntVec3 tgt = pawn.Map.cellIndices.IndexToCell(0);

            // There's probably a cleaner way to do this. I'm too tired to figure one out.
            switch (missDir)
            {
                case 0:
                    tgt = TargetB.Cell;
                    tgt.x += 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 1;
                case 1:
                    tgt = TargetB.Cell;
                    tgt.x += 1;
                    tgt.z += 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 2;
                case 2:
                    tgt = TargetB.Cell;
                    tgt.z += 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 3;
                case 3:
                    tgt = TargetB.Cell;
                    tgt.x -= 1;
                    tgt.z += 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 4;
                case 4:
                    tgt = TargetB.Cell;
                    tgt.x -= 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 5;
                case 5:
                    tgt = TargetB.Cell;
                    tgt.x -= 1;
                    tgt.z -= 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 6;
                case 6:
                    tgt = TargetB.Cell;
                    tgt.z -= 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    goto case 7;
                case 7:
                    tgt = TargetB.Cell;
                    tgt.x += 1;
                    tgt.z -= 1;
                    if (tgt.InBounds(pawn.Map))
                        break;
                    tgt = pawn.Map.cellIndices.IndexToCell(0);
                    break;
            }

            hitTarget = tgt;
        }
        else
        {
            Log.Message(
                $"RATS attack Hit to {attack.Target} on {attack.Part} with hit chance {attack.HitChance}"
            );
            hitTarget = TargetB;
        }

        ProjectileHitFlags hitFlags1 = ProjectileHitFlags.IntendedTarget;

        projectile2.Launch(
            pawn,
            pawn.DrawPos,
            hitTarget,
            TargetB,
            hitFlags1,
            false,
            attack.Equipment
        );
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
        Toil init = ToilMaker.MakeToil();
        init.initAction = () =>
        {
            if (TargetThingA is Pawn targetThingA2)
                startedIncapacitated = targetThingA2.Downed;
            pawn.pather.StopDead();
        };
        init.tickAction = () =>
        {
            if (!TargetA.IsValid)
            {
                EndJobWith(JobCondition.Succeeded);
            }
            else
            {
                if (TargetA.HasThing)
                {
                    Pawn thing = TargetA.Thing as Pawn;
                    if (
                        TargetA.Thing.Destroyed
                        || thing != null && !startedIncapacitated && thing.Downed
                        || thing != null && thing.IsPsychologicallyInvisible()
                    )
                    {
                        EndJobWith(JobCondition.Succeeded);
                        return;
                    }
                }
                if (hasAttacked)
                    EndJobWith(JobCondition.Succeeded);
                else if (TryStartAttack(TargetA))
                {
                    hasAttacked = true;
                }
                else
                {
                    if (pawn.stances.FullBodyBusy)
                        return;
                    Verb attackVerb = pawn.TryGetAttackVerb(TargetA.Thing, !pawn.IsColonist);
                    if (
                        job.endIfCantShootTargetFromCurPos
                        && (
                            attackVerb == null
                            || !attackVerb.CanHitTargetFrom(pawn.Position, TargetA)
                        )
                    )
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                    else
                    {
                        if (!job.endIfCantShootInMelee)
                            return;
                        if (attackVerb == null)
                        {
                            EndJobWith(JobCondition.Incompletable);
                        }
                        else
                        {
                            float num = attackVerb.verbProps.EffectiveMinRange(TargetA, pawn);
                            IntVec3 position1 = pawn.Position;
                            LocalTargetInfo targetA = TargetA;
                            IntVec3 cell1 = targetA.Cell;
                            if (position1.DistanceToSquared(cell1) >= num * (double)num)
                                return;
                            IntVec3 position2 = pawn.Position;
                            targetA = TargetA;
                            IntVec3 cell2 = targetA.Cell;
                            if (!position2.AdjacentTo8WayOrInside(cell2))
                                return;
                            EndJobWith(JobCondition.Incompletable);
                        }
                    }
                }
            }
        };
        init.defaultCompleteMode = ToilCompleteMode.Never;
        init.activeSkill = () => Toils_Combat.GetActiveSkillForToil(init);
        yield return init;
    }
}
