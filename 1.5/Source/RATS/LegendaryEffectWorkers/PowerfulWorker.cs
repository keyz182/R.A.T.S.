using System;
using System.Reflection;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class PowerfulWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo)
    {
        Type dType = typeof(DamageInfo);
        FieldInfo amountInt = dType.GetField("amountInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (amountInt != null)
        {
            float damageAmount = (float)amountInt.GetValue(damageInfo);
            amountInt.SetValueDirect(__makeref(damageInfo), damageAmount * 1.25f);
        }
    }
}
