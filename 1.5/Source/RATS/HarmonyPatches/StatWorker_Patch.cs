using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS.HarmonyPatches;

[HarmonyPatch(typeof(StatWorker))]
public static class StatWorker_Patch
{
    [HarmonyPatch(nameof(StatWorker.GetValueUnfinalized))]
    [HarmonyPostfix]
    public static void GetValueUnfinalized_Patch(StatWorker __instance, ref float __result, StatRequest req, bool applyPostProcess = true)
    {
        if (req.Thing is Pawn thing1)
        {
            StatDef statDef = Traverse.Create(__instance).Field("stat").GetValue<StatDef>();
            if (thing1.apparel != null)
            {
                for (int index = 0; index < thing1.apparel.WornApparel.Count; ++index)
                    __result += StatOffset(thing1.apparel.WornApparel[index], statDef);
            }

            if (thing1.equipment?.Primary != null)
                __result += StatOffset(thing1.equipment.Primary, statDef);
        }
    }

    public static float StatOffset(Thing gear, StatDef statDef)
    {
        if (!LegendaryEffectGameTracker.HasEffect(gear))
        {
            return 0;
        }

        IEnumerable<StatModifier> statMod = LegendaryEffectGameTracker.EffectsDict[gear].Where(eff => !eff.StatModifiers.NullOrEmpty()).SelectMany(eff => eff.StatModifiers);

        statMod = statMod.Where(stat => stat.stat == statDef);

        float statOffset = 0;

        foreach (StatModifier statModifier in statMod)
        {
            float statOffsetFromList = statModifier.value;
            if (Mathf.Approximately(statModifier.value, 0) && !statDef.parts.NullOrEmpty<StatPart>())
            {
                foreach (StatPart part in statDef.parts)
                {
                    part.TransformValue(StatRequest.For(gear), ref statOffsetFromList);
                }
            }
            statOffset += statOffsetFromList;
        }

        return statOffset;
    }
}
