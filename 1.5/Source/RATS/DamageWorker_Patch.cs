using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace RATS;

[HarmonyPatch(typeof(DamageWorker_AddInjury))]
public static class DamageWorker_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch("ApplyToPawn")]
    public static bool ApplyToPawn_Patch(ref DamageInfo dinfo, Pawn pawn)
    {
        if (!RATS_GameComponent.ActiveAttacks.TryGetValue((Pawn)dinfo.Instigator, out RATS_GameComponent.RATSAction attack) || attack.Target != pawn)
        {
            return true;
        }

        Type dType = typeof(DamageInfo);

        dType.GetField("hitPartInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValueDirect(__makeref(dinfo), attack.Part);
        dType
            .GetField("allowDamagePropagationInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValueDirect(__makeref(dinfo), false);

        return true;
    }
}
