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
        harmonyInstance = new Harmony("keyz182.RATS");

        harmonyInstance.Patch(
            AccessTools.PropertyGetter(typeof(ShaderTypeDef), nameof(ShaderTypeDef.Shader)),
            new HarmonyMethod(typeof(RATSMod), nameof(RATSMod.ShaderFromAssetBundle))
        );

        harmonyInstance.PatchAll();
    }
}
