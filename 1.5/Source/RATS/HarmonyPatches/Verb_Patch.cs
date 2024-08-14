using HarmonyLib;
using Verse;

namespace RATS.HarmonyPatches;

[HarmonyPatch(typeof(Verb))]
public static class Verb_Patch
{
    [HarmonyPatch(nameof(Verb.OutOfRange))]
    [HarmonyPrefix]
    public static bool OutOfRange_Patch(Verb __instance, ref bool __result, IntVec3 root, LocalTargetInfo targ, CellRect occupiedRect)
    {
        Verb_AbilityRats rats = __instance as Verb_AbilityRats;

        if (rats == null)
            return true;

        float minRange = rats.PrimaryWeaponVerbProps.EffectiveMinRange(targ, rats.caster);
        float distance = occupiedRect.ClosestDistSquaredTo(root);
        __result = distance > rats.EffectiveRange * (double)rats.EffectiveRange || distance < minRange * (double)minRange;

        return false;
    }
}
