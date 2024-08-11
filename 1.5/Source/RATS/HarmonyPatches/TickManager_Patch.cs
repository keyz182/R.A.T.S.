﻿using HarmonyLib;
using Verse;

namespace RATS.HarmonyPatches;

[HarmonyPatch(typeof(TickManager))]
[HarmonyPatch("TickRateMultiplier", MethodType.Getter)]
public static class TickManagerPatch
{
    [HarmonyPrefix]
    private static bool ModifyTickRate(ref float __result)
    {
        TimeSlower slower = Find.TickManager.slower;
        TimeSpeed curTimeSpeed = Find.TickManager.CurTimeSpeed;

        if (!RATS_GameComponent.SlowMoActive)
        {
            return true;
        }

        if (slower.ForcedNormalSpeed && curTimeSpeed == TimeSpeed.Paused)
        {
            __result = 0f;
        }
        else
        {
            __result = 0.25F;
        }

        return false;
    }
}
