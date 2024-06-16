using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RATS;

[HarmonyPatch(typeof(Thing))]
public static class Thing_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Thing.TakeDamage))]
    public static bool TakeDamage_Patch(Thing __instance, ref DamageWorker.DamageResult __result, DamageInfo dinfo)
    {
        // if (__instance is Pawn target && dinfo.Instigator is Pawn instigator)
        // {
        //     var action = RATS_GameComponent.ActiveAttacks.GetValueOrDefault(instigator, null);
        //     if (action != null && action.Target == target)
        //     {
        //         Log.Message("here");
        //         if (__instance.Destroyed || dinfo.Amount == 0.0)
        //         {
        //             __result = new DamageWorker.DamageResult();
        //             return false;
        //         }
        //
        //         if (__instance.def.damageMultipliers != null)
        //         {
        //             for (int index = 0; index < __instance.def.damageMultipliers.Count; ++index)
        //             {
        //                 if (__instance.def.damageMultipliers[index].damageDef == dinfo.Def)
        //                 {
        //                     int newAmount = Mathf.RoundToInt(dinfo.Amount * __instance.def.damageMultipliers[index].multiplier);
        //                     dinfo.SetAmount((float) newAmount);
        //                 }
        //             }
        //         }
        //         
        //         bool absorbed;
        //         __instance.PreApplyDamage(ref dinfo, out absorbed);
        //         if (absorbed)
        //         {
        //             __result = new DamageWorker.DamageResult();
        //             return false;
        //         }
        //     }
        // }
        
        
        Log.Message("here");
        return true;
    }
    
}