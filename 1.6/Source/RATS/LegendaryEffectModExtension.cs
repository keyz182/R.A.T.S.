using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RATS;

public class LegendaryEffectModExtension : DefModExtension
{
    public List<LegendaryEffectDef> legendaryEffects;

    public static LegendaryEffectModExtension RandomLegendaryFor(ThingDef thingDef)
    {
        LegendaryEffectModExtension ext = new LegendaryEffectModExtension();
        ext.AddNewLegendaryEffectFor(thingDef);

        return ext;
    }

    public void AddNewLegendaryEffectFor(ThingDef thingDef)
    {
        var AllDefs = DefDatabase<LegendaryEffectDef>.AllDefsListForReading;
        LegendaryEffectDef effect = AllDefs.Where(def => def.IsForApparel == thingDef.IsApparel || def.IsForWeapon == thingDef.IsWeapon).RandomElement();
        if (legendaryEffects.NullOrEmpty())
        {
            legendaryEffects = new List<LegendaryEffectDef>();
        }
        legendaryEffects.Add(effect);
    }
}
