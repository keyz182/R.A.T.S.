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

    public AssetBundle MainBundle
    {
        get
        {
            string text = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                text = "StandaloneOSX";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = "StandaloneWindows64";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                text = "StandaloneLinux64";
            string bundlePath = Path.Combine(Content.RootDir, $@"Materials\Bundles\{text}\zoom");
            Log.Message("Bundle Path: " + bundlePath);

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
                Log.Error("Failed to load bundle at path: " + bundlePath);

            foreach (string allAssetName in bundle.GetAllAssetNames())
                Log.Message($"[{allAssetName}]");

            return bundle;
        }
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

    public static void ShaderFromAssetBundle(ShaderTypeDef __instance, ref Shader ___shaderInt)
    {
        if (__instance is not RATSShaderTypeDef)
            return;
        ___shaderInt = RATS_Shaders.AssetBundle.LoadAsset<Shader>(__instance.shaderPath);
        if (___shaderInt is null)
            Log.Error($"Failed to load Shader from path <text>\"{__instance.shaderPath}\"</text>");
    }
}
