﻿using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class ExplosiveWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo)
    {
        Effecter eff = RATS_DefOf.Rats_LegendaryEffect_Explosive_Explosion.Spawn();
        damageInfo.Instigator.Map.effecterMaintainer.AddEffecterToMaintain(eff, damageInfo.IntendedTarget.Position.ToVector3().ToIntVec3(), 60);

        GenExplosion.DoExplosion(
            damageInfo.IntendedTarget.Position,
            damageInfo.Instigator.Map,
            4,
            DamageDefOf.Bomb,
            damageInfo.Instigator,
            ignoredThings: [damageInfo.Instigator],
            propagationSpeed: 0.5f
        );
    }
}
