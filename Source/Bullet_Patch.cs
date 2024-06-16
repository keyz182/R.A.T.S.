using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using CodeInstruction = HarmonyLib.CodeInstruction;

namespace RATS;

[HarmonyPatch(typeof(Bullet))]
public static class Bullet_Patch
{
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Bullet.Impact))]
    public static IEnumerable<CodeInstruction> Impact_Patch(IEnumerable<CodeInstruction> instructions)
    {
        
        return instructions.AsEnumerable();
    }
    
}