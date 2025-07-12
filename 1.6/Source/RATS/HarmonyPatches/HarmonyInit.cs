using HarmonyLib;
using Verse;

namespace RATS.HarmonyPatches;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    public static Harmony harmonyInstance;

    static HarmonyInit()
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif
        harmonyInstance = new Harmony("keyz182.RATS");
        harmonyInstance.PatchAll();
    }
}
