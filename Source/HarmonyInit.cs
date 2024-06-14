using HarmonyLib;
using Verse;

namespace RATS;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    public static Harmony harmonyInstance;

    static HarmonyInit()
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif
        harmonyInstance = new Harmony("RATS.Mod");
        harmonyInstance.PatchAll();
    }
}
