using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS.HarmonyPatches;

[HarmonyPatch(typeof(Pawn))]
public static class Pawn_Patch
{
    [HarmonyPatch(nameof(Thing.SpecialDisplayStats))]
    [HarmonyPostfix]
    public static void SpecialDisplayStats_Postfix(Pawn __instance, ref IEnumerable<StatDrawEntry> __result)
    {
        List<LegendaryEffectDef> effects = new();

        if (__instance.apparel != null)
        {
            foreach (Apparel apparel in __instance.apparel.WornApparel)
            {
                if (LegendaryEffectGameTracker.HasEffect(apparel))
                    effects.AddRange(LegendaryEffectGameTracker.EffectsDict[apparel]);
            }
        }

        if (__instance.equipment?.Primary != null)
            if (LegendaryEffectGameTracker.HasEffect(__instance.equipment.Primary))
                effects.AddRange(LegendaryEffectGameTracker.EffectsDict[__instance.equipment.Primary]);

        List<StatDrawEntry> output = __result.ToList();

        foreach (LegendaryEffectDef effectDef in effects)
        {
            foreach (StatModifier statMod in effectDef.StatModifiers)
            {
                StatDef stat = statMod.stat;
                StringBuilder stringBuilder = new StringBuilder();
                float val = statMod.value;

                if (stat.Worker != null)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("StatsReport_BaseValue".Translate() + ": " + stat.ValueToString(val, ToStringNumberSense.Offset, stat.finalizeEquippedStatOffset));
                    val = StatWorker.StatOffsetFromGear(__instance, stat);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("StatsReport_FinalValue".Translate() + ": " + stat.ValueToString(val, ToStringNumberSense.Offset, !stat.formatString.NullOrEmpty()));
                }

                output.Add(
                    new LegendaryStatDrawEntry(
                        effectDef,
                        RATS_DefOf.Rats_LegendaryEffect_StatCat,
                        stat,
                        statMod.value,
                        StatRequest.ForEmpty(),
                        ToStringNumberSense.Offset,
                        forceUnfinalizedMode: true
                    ).SetReportText(stringBuilder.ToString())
                );
            }
        }

        __result = output.AsEnumerable();
    }
}
