using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Verse;

namespace RATS;

public class RATSMod : Mod
{
    public static RATS_Settings Settings;

    public static RATSMod mod;

    public RATSMod(ModContentPack content)
        : base(content)
    {
        mod = this;
        Settings = GetSettings<RATS_Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "RATS_Settings_Category".Translate();
    }
}
