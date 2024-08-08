using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace RATS;

[HarmonyPatch(typeof(TickManager), "TickManagerUpdate")]
public static class TickManagerUpdate_Transpiler_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction inst in instructions)
        {
            if (
                inst.opcode == OpCodes.Call
                && ((MethodInfo)inst.operand).FullDescription()
                    == "System.Single Verse.TickManager::get_TickRateMultiplier()"
            )
            {
                MethodInfo newOp = AccessTools.Method(
                    typeof(RATS_GameComponent),
                    nameof(RATS_GameComponent.TickRateMultiplier)
                );
                yield return new CodeInstruction(OpCodes.Call, newOp);
            }
            else
            {
                yield return inst;
            }
        }
    }
}
