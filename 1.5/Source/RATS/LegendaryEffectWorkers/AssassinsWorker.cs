using System;
using System.Reflection;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class AssassinsWorker : LegendaryEffectWorker
{
    public override void ApplyToDamageInfo(ref DamageInfo damageInfo)
    {
        if (damageInfo.IntendedTarget is Pawn pawn)
        {
            Type dType = typeof(DamageInfo);
            FieldInfo amountInt = dType.GetField("amountInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (amountInt != null)
            {
                float damageAmount = (float)amountInt.GetValue(damageInfo);
                amountInt.SetValueDirect(__makeref(damageInfo), damageAmount * 1.5f);
            }
        }
    }
}
