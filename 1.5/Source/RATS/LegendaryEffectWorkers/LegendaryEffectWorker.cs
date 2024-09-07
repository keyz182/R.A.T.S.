﻿using Verse;

namespace RATS.LegendaryEffectWorkers;

public class LegendaryEffectWorker
{
    public LegendaryEffectDef effect;

    public void InitSetEffect(LegendaryEffectDef newEffect) => this.effect = newEffect;

    public virtual void ApplyEffect(ref DamageInfo damageInfo) { }
}