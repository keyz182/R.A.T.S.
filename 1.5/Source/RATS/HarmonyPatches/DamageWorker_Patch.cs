using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RATS.HarmonyPatches;

[HarmonyPatch(typeof(DamageWorker_AddInjury))]
public static class DamageWorker_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch("ApplyToPawn")]
    public static bool ApplyToPawn_Patch(ref DamageInfo dinfo, Pawn pawn)
    {
        if (dinfo.Instigator is not Pawn instigator)
        {
            return true;
        }

        if (RATS_GameComponent.ActiveAttacks.TryGetValue(instigator, out RATS_GameComponent.RATSAction attack) && attack.Target == pawn)
        {
            Type dType = typeof(DamageInfo);

            dType.GetField("hitPartInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValueDirect(__makeref(dinfo), attack.Part);
            dType
                .GetField("allowDamagePropagationInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValueDirect(__makeref(dinfo), false);
        }

        foreach (LegendaryEffectDef legendaryEffectDef in GetLegendaryEffectsFor(instigator))
        {
            legendaryEffectDef.Worker.ApplyEffect(ref dinfo, pawn);
        }
        return true;
    }

    public static IEnumerable<LegendaryEffectDef> GetLegendaryEffectsFor(Pawn pawn)
    {
        if (
            pawn.equipment?.Primary != null
            && pawn.equipment.Primary.TryGetQuality(out QualityCategory quality)
            && quality == QualityCategory.Legendary
            && LegendaryEffectGameTracker.HasEffect(pawn.equipment.Primary)
        )
        {
            foreach (LegendaryEffectDef effectDef in LegendaryEffectGameTracker.EffectsDict[pawn.equipment.Primary])
            {
                yield return effectDef;
            }
        }

        if (pawn.apparel != null)
        {
            foreach (
                Apparel apparel in pawn
                    .apparel.WornApparel.Where(app => app.TryGetQuality(out QualityCategory qual) && qual == QualityCategory.Legendary)
                    .Where(LegendaryEffectGameTracker.HasEffect)
            )
            {
                foreach (LegendaryEffectDef effectDef in LegendaryEffectGameTracker.EffectsDict[apparel])
                {
                    yield return effectDef;
                }
            }
        }
    }
}
