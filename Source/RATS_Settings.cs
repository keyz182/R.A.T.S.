using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class RATS_Settings : ModSettings
{
    public float MaxGain =>
        1f
        + Enumerable
            .Range(0, 20)
            .Sum(i => i == 0 ? 0 : Mathf.Pow(RATSMod.Settings.GainPerLevel, i));
    public float GainPerLevel = 0.15f;
    public float ClampingFactor = 0.5f;

    public float GetClampedValue(int level)
    {
        return (MaxGain / ClampingFactor) * GetScaledValue(level);
    }

    public float GetScaledValue(int level)
    {
        return Enumerable
            .Range(0, level)
            .Sum(i => i == 0 ? 0 : Mathf.Pow(RATSMod.Settings.GainPerLevel, i));
    }

    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new();
        options.Begin(wrect);

        options.Label(
            "RATS_Settings_Gain_Per_Level".Translate(GainPerLevel.ToString("F5")),
            tooltip: "RATS_Settings_Gain_Per_Level_Mouseover".Translate()
        );
        GainPerLevel = options.Slider(GainPerLevel, 0.01f, 4f);
        options.Gap();

        options.Label(
            "RATS_Settings_ClampingFactor".Translate(ClampingFactor.ToString("F5")),
            tooltip: "RATS_Settings_ClampingFactor".Translate()
        );
        ClampingFactor = options.Slider(ClampingFactor, 0.01f, 2f);
        options.Gap();

        if (options.ButtonText("RATS_Settings_Reset".Translate()))
        {
            GainPerLevel = 0.15f;
            ClampingFactor = 0.15f;
        }
        options.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref GainPerLevel, "GainPerLevel", 0.15f);
    }
}
