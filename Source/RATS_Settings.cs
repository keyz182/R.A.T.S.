using UnityEngine;
using Verse;

namespace RATS;

public class RATS_Settings : ModSettings
{
    public float GainPerLevel = 0.15f;
    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new();
        options.Begin(wrect);

        options.Label("RATS_Settings_Gain_Per_Level".Translate(GainPerLevel.ToString("F5")));
        GainPerLevel = options.Slider(GainPerLevel, 0.01f, 0.5f);
        options.Gap();
        
        if (options.ButtonText("RATS_Settings_Reset".Translate()))
        {
            GainPerLevel = 0.15f;
        }
        options.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref GainPerLevel, "GainPerLevel", 0.15f);
    }
}