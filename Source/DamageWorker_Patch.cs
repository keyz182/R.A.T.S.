using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace RATS;

[HarmonyPatch(typeof(DamageWorker_AddInjury))]
public static class DamageWorker_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DamageWorker_AddInjury.ApplyToPawn))]
    public static bool ApplyToPawn_Patch(ref DamageInfo dinfo, Pawn pawn)
    {
        var attack = RATS_GameComponent.ActiveAttacks.GetValueOrDefault((Pawn)dinfo.Instigator);

        if (attack == null || attack.Target != pawn) return true;

        var dType = typeof(DamageInfo);
        
        dType.GetField("hitPartInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValueDirect(__makeref(dinfo), attack.Part);
        dType.GetField("allowDamagePropagationInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValueDirect(__makeref(dinfo), false);
        
        return true;
    }
    
}