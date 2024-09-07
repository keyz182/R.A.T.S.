using System;
using System.Collections.Generic;
using RATS.LegendaryEffectWorkers;
using RimWorld;
using Verse;

namespace RATS;

public class LegendaryEffectDef : Def
{
    public List<StatModifier> StatModifiers;
    public List<CompProperties> AdditionalComps;
    public bool IsForWeapon = false;
    public bool IsForMelee = false;
    public bool IsForApparel = false;
    public Type workerClass = typeof(LegendaryEffectWorker);

    [Unsaved(false)]
    private LegendaryEffectWorker workerInt;

    public LegendaryEffectWorker Worker
    {
        get
        {
            if (workerInt == null)
            {
                workerInt = (LegendaryEffectWorker)Activator.CreateInstance(workerClass);
                workerInt.InitSetEffect(this);
            }
            return workerInt;
        }
    }
}
