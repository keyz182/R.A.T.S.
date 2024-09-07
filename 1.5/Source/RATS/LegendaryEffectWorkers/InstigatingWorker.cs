using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class InstigatingWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo)
    {
        if (damageInfo.IntendedTarget is Pawn pawn && Mathf.Approximately(pawn.health.summaryHealth.SummaryHealthPercent, 0f))
        {
            Type dType = typeof(DamageInfo);
            FieldInfo amountInt = dType.GetField("amountInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (amountInt != null)
            {
                float damageAmount = (float)amountInt.GetValue(damageInfo);
                amountInt.SetValueDirect(__makeref(damageInfo), damageAmount * 2f);
            }
        }
    }
}
