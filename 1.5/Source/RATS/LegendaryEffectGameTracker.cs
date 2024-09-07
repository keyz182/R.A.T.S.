using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Verse;

namespace RATS;

public class LegendaryEffectGameTracker : GameComponent
{
    public LegendaryEffectGameTracker(Game game) { }

    public static Dictionary<Thing, List<LegendaryEffectDef>> EffectsDict = new Dictionary<Thing, List<LegendaryEffectDef>>();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref EffectsDict, "EffectsMap", LookMode.Reference, LookMode.Def);
    }

    public static void AddNewLegendaryEffectFor(Thing thing)
    {
        List<LegendaryEffectDef> AllDefs = DefDatabase<LegendaryEffectDef>.AllDefsListForReading;
        LegendaryEffectDef effect;

        if (thing.def.IsApparel)
        {
            effect = AllDefs.Where(def => def.IsForApparel).RandomElement();
        }
        else if (thing.def.IsWeapon)
        {
            if (thing.def.weaponClasses.Any(cls => cls.defName.ToLower().Contains("melee")))
            {
                effect = AllDefs.Where(def => def.IsForWeapon && def.IsForMelee).RandomElement();
            }
            else
            {
                effect = AllDefs.Where(def => def.IsForWeapon && !def.IsForMelee).RandomElement();
            }
        }
        else
        {
            return;
        }

        if (!EffectsDict.TryGetValue(thing, out var effects))
            effects = new List<LegendaryEffectDef>();

        effects.Add(effect);

        EffectsDict.SetOrAdd(thing, effects);
    }

    public static bool HasEffect(Thing thing)
    {
        return EffectsDict.ContainsKey(thing) && EffectsDict[thing].Count > 0;
    }

    public static string GetEffectDescription(Thing thing)
    {
        if (!HasEffect(thing))
        {
            return null;
        }

        StringBuilder body = new StringBuilder();
        foreach (LegendaryEffectDef effect in EffectsDict[thing])
        {
            body.AppendLine($" - {effect.LabelCap} - {effect.description}");
        }

        return RemoveEmptyLines(body.ToString());
    }

    public static string RemoveEmptyLines(string lines)
    {
        return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
    }
}
